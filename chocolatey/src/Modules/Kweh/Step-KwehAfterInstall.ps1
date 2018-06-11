function Step-KwehAfterInstall() {
    Param(
        [Parameter(Position = 0)]
        [PsCustomObject] $Config,

        [Parameter(Position = 1, ValueFromPipeline = $true)]
        [PsCustomObject] $Context
    )   
    
    if($Context.Usb) {
        $instructions = $Config.Usb
        $installDir = $Context.Destination
        $tmp = $Context.Tmp 
        $installLabel = $Context.InstallLabel 

        Write-Host "Temp $tmp"
        Write-Host "Label $installLabel"
        $set = @{}
        $instructions | Get-Member -MemberType NoteProperty | Foreach-object {
            $name =$_.Name 
            $list = $instructions.$name
            $set.Add($name, $list)
        }
        $isAdmin = Test-IsElevated 
        $order = @("mk", "mv","cp", "ln", "scripts")

        foreach($name in $order) {
            if(!$set.ContainsKey($name)) {
                continue;
            }

            $list = $set[$name]


            Switch($Name) {
                "mk" {
                    foreach($directive in $list) {
                        if($directive.dir) {
                            $dir = $Context | Resolve-StringTemplate $directive.dir
                            if($dir.StartsWith("./")) {
                                $dir = $installDir + "/" + $dir.SubString(2)
                            }
                            if(-not (Test-Path $dir)) {
                                Write-Output $dir
                                New-Item $dir -ItemType Directory | Write-Debug
                            }
                        }
                    }
                }
                "mv" {
                    foreach($directive in $list) {
                        $src = $directive.src
                        $dest = $directive.dest
                        if(!$src -or !$dest) {
                            write-host "missing entry src $src / dest $dest"
                        }

                        if($src.StartsWith("./")) {
                            $src = $src.Substring(2)
                            $src = "$installDir/$src"
                        }
                        
                        if($dest.StartsWith("./")) {
                            $dest = $dest.Substring(2)
                            $dest = "$installDir/$dest"
                        }

                        $src = gi ($Context | Resolve-StringTemplate $src)
                        $dest = ($Context | Resolve-StringTemplate $dest)
                       
                        if($src) {
                            $src = $src.FullName.Replace("\", "/")
                        }
                        $link = $directive.link
                        $force = $directive.force 

                        $isFile = !(Test-Path $src -PathType Container);
                        
                        if($force -or $isFile -or !(Test-Path $dest)) {
                            if(Test-Path $dest) {
                                Remove-Item $dest -Force -Recurse 
                            }
                             
                           
                            Move-Item -Path $src -Destination $dest -Force
                            
                            if($link) {
                                
                                New-HardLink -Path $dest -Link $src 
                            }
                            continue; 
                        }
                        
                        $items = gci $src -Recurse
                        
                        foreach($item in $items) {
                            $filepath = $item.FullName   
                            $relative = $filepath.SubString($src.Length + 1).Replace("\", "/")
                            Write-Host "$relative"
                            if(-not (Test-Path "$dest/$relative")) {
                                Copy-Item $filepath "$dest/$relative" -Force  
                            } else {
                                $leaf = Split-Path $src -Leaf 
                                if(-not (Test-Path "$tmp/$installLabel/$leaf")) {
                                    New-Item "$tmp/$installLabel/$leaf" -ItemType Directory | Write-Debug 
                                }
                                Copy-Item $filepath "$tmp/$installLabel/$leaf/$relative" -Force
                            }
                        }
                        Remove-Item $src -Force -Recurse
                        if($link) {
                            New-HardLink -Path $dest -Link $src 
                        } 
                    }
                }
                "cp" {
                    foreach($directive in $list) {
                        $src = $directive.src
                        if($src.StartsWith("./")) {
                            $src = $src.SubString(2)
                            $src = "$installDir/$src"
                        }
                        $src = gi ($Context | Resolve-StringTemplate $directive.src) -ErrorAction SilentlyContinue

                        if($src) {
                            $dest = $Context | Resolve-StringTemplate $directive.dest 
                            $force = $directive.force
                            $args = @{
                                Path = $src
                                Destination = $dest
                                Force = $force 
                            }
                            Copy-Item @args   
                        } else {
                            Write-Warning "Could not locate $src"
                        }
                        
                       
                    }
                     
                }
                "ln" {
                    foreach($directive in $list) {
                        $src = $directive.src
                        $dest = $directive.dest

                        if(!$src -or !$dest) {
                            continue;
                        }

                        if($src.StartsWith("./")) {
                            $src = $src.SubString(2)
                            $src = "$installDir/$src"
                        }

                        
                        if($dest.StartsWith("./")) {
                            $dest = $dest.SubString(2)
                            $dest = "$installDir/$dest"
                        }
                        $src = ($Context | Resolve-StringTemplate $src) 
                        $dest = $Context | Resolve-StringTemplate $dest 
                        if(Test-Path $dest) {
                            Remove-Item $dest -Force | Write-Debug
                        }
                       
                        New-HardLink -Path $src -Link $dest
                    }
                }
                "env" {
                    foreach($directive in $list) {
                        $name = $directive.name 
                        $scope = $directive.scope
                        $value = $directive.value 
                        $value = Resolve-KwehStringTemplate $value $context 
                        if($scope -ne "machine" -or $scope -ne "user" -or $scope -ne "process") {
                            $scope = "machine"
                        }
                        if(!$isAdmin -and $scope -eq "machine") {
                            $scope = "user"
                        }

                        [Environment]::SetEnvironmentVariable($name, $value, $scope) 
                    }
                }
                "scripts" {
                    foreach($directive in $list) {
                        $script = $Context | Resolve-StringTemplate $directive
                        if($script.StartsWith("./")) {
                            $script = $script.Substring(2)
                            $script = ($Context.PackageDir.Replace("\", "/")) + "/$script" 
                        }
                        & $script -Config $Config -Context $Context;
                    }
                }
            }
        }

        if($Context.afterInstall -and $Context.afterInstall -is [Array]) {
            foreach($directive in $Context.afterInstall) {
                $script = $Context | Resolve-StringTemplate $directive;
                if($script.StartsWith("./")) {
                    $script = $script.Substring(2)
                    $script = ($Context.PackageDir.Replace("\", "/")) + "/$script" 
                }
                & $script -Config $Config -Context $Context;
            }
        }
    }
}

Set-Alias -Name Step-AfterInstall -Value Step-KwehAfterInstall  