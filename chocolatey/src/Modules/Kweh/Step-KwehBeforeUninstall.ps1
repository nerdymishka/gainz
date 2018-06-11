function Step-KwehBeforeUninstall() {
    Param(
        [Parameter(Position = 0)]
        [PsCustomObject] $Config,

        [Parameter(Position = 1, ValueFromPipeline = $true)]
        [PsCustomObject] $Context 
    )

    if($Config.Services) {
        foreach($svc in $config.Services) {

            $src = $svc.src 
            $src = $directive.src;
            if($src.StartsWith("./")) {
                $src = $src.Substring(2)
                $src = "$installDir/$src"
            }
            $src = gi ($Context | Resolve-StringTemplate $src) -EA SilentlyContinue

            if(!$svc.Name) {
                Stop-Service ($svc.Name) -Force 
                Start-Sleep 1  
            }

            if($svc.type -eq "topshelf") {
                & $src uninstall 
            }
            if($svc.type -eq "script") {
                $script = $svc.script 
            }
            if($svc.type -eq "dotnet") {
            
                if($svc.x64 -and [IntPtr]::Zero -eq 8) {
                    $installUtil = "C:\Windows\Microsoft.NET\Framework64\v4.0.30319\InstallUtil.exe"
                } else {
                    $installUtil = "C:\Windows\Microsoft.NET\Framework\v4.0.30319\InstallUtil.exe"
                }
                
                & $installutil /u $src.FullName 
            }
        }
    }

    if($Context.OptInstall) {
        $instructions = $Config.Opt 
        $installDir = $Context.Destination
        $tmp = $Context.Tmp 
        $installLabel = $Context.InstallLabel 

       
        if(Test-Path "$installDir/bin") {
            $binaries = Get-ChildItem "$installDir/bin" -Filter *.exe 
            if($binaries) {
                foreach($bin in $binaries) {
                   $processes =  Get-Process ($bin.Name) -EA SilentlyContinue
                   if($processes) {
                       if(!($processes -is [Array])) {
                           $processes = @($processes)
                       }
                       foreach($process in $processes) {
                           if($process.Path -eq $bin.FullName) {
                               $process.Kill()
                           }
                       }
                   } 
                }
            }

            $binaries = Get-ChildItem "$installDir" -Filter *.exe 
            if($binaries) {
                foreach($bin in $binaries) {
                   $processes =  Get-Process ($bin.Name) -EA SilentlyContinue
                   if($processes) {
                       if(!($processes -is [Array])) {
                           $processes = @($processes)
                       }
                       foreach($process in $processes) {
                           if($process.Path -eq $bin.FullName) {
                               $process.Kill()
                           }
                       }
                   } 
                }
            }
        }
    

     
        $set = @{}
        $instructions | Get-Member -MemberType NoteProperty | Foreach-object {
            $name =$_.Name 
            $list = $instructions.$name
            $set.Add($name, $list)
        }

        $order = @("mk", "mv", "ln","scripts")

        foreach($name in $order) {
            if(!$set.ContainsKey($name)) {
                continue;
            }

            $list = $set[$name]


            Switch($Name) {
                "mk" {
                    foreach($directive in $list) {
                        if($directive.dir -and $directive.rm -or $directive.remove) {
                            $dir = $Context | Resolve-StringTemplate $directive.dir
                            if($dir.StartsWith("./")) {
                                $dir = $installDir + "/" + $dir.SubString(2)
                            }
                            if((Test-Path $dir)) {
                                Remove-Item $dir -Recurse -Force | Write-Debug
                            }
                        }
                    }
                }
                "mv" {
                    foreach($directive in $list) {
                        $link = $directive.link 
                        $rm = if($directive.rm -or $directive.remove) { $true }else { $false }
                        if($link) {
                            $src = $directive.src;
                            if($src.StartsWith("./")) {
                                $src = $src.Substring(2)
                                $src = "$installDir/$src"
                            }
                            $src = gi ($Context | Resolve-StringTemplate $src) -EA SilentlyContinue
                            if($src) {
                                Remove-Item ($src.FullName) -Force 
                            }
                        }
                        
                        if($rm) {
                            $dest = $directive.dest
                    
                            if([string]::IsNullOrWhiteSpace($dest)) {
                                Write-Warning "missing dest property for mv entry"
                                continue
                            }
                                
                                
                            
                            if($dest.StartsWith("./")) {
                                $dest = $dest.Substring(2)
                                $dest = "$installDir/$dest"
                            }
                                
                            $dest = gi ($Context | Resolve-StringTemplate $dest) -EA SilentlyContinue
                            if($dest) {
                                Remove-Item $dest -Force -Recurse | Write-Debug
                            }
                        }
                    }
                }
                "cp" {  }
                "ln" {
                    foreach($directive in $list) {
                        $src = $directive.src
                        $dest = $directive.dest

                        if(!$dest) {
                            continue;
                        }

                        if($dest.StartsWith("./")) {
                            $dest = $dest.SubString(2)
                            $dest = "$installDir/$dest"
                        }
                       
                        $dest = gi ($Context | Resolve-StringTemplate $dest) -EA SilentlyContinue
                        if(!$dest) {
                            continue;
                        }
                    
                        Remove-Item $dest.FullName -Force | Write-Debug
                       
                       
                        
                    }
                }
                "env" {
                    foreach($directive in $list) {
                        $name = $directive.name 
                        $scope = $directive.scope
                       
                        if($scope -ne "machine" -or $scope -ne "user" -or $scope -ne "process") {
                            $scope = "machine"
                        }
                        if(!$isAdmin -and $scope -eq "machine") {
                            $scope = "user"
                        }

                        [Environment]::SetEnvironmentVariable($name, $null, $scope) 
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

        if($Context.beforeUinstall -and $Context.beforeUninstall -is [Array]) {
            foreach($directive in $Context.beforeUninstall) {
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

Set-Alias -Name Step-BeforeUninstall -Value Step-KwehBeforeUninstall