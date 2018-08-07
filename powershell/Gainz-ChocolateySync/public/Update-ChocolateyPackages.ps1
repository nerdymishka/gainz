function Update-ChocolateyPackages() {
    Param(
        [PSCustomObject] $Config
    )
    Write-Banner 
    $choco = Get-Command choco -ErrorAction SilentlyContinue
    if(!$choco) {
    
        return;
    }
    $choco = $choco.Path;
    $statePath = "$HOME/.config/nerdymishka/chocolatey/state.text"
    $state = $false;
    if(Test-Path $statePath) {
        $state = Get-Content -Raw $statePath
    }
    $canDetectReboot = ($null -ne (Get-Command  Test-PendingReboot -ErrorAction SilentlyContinue))
    $canDecrypt = ($null -ne (Get-Command Unprotect-String -ErrorAction SilentlyContinue))
    $update = $config.update;
    if(!$update) {
        $update = $false;
    }

    if(!$config.packages) {
        Write-Warning "Config.packages does not exist"
        return;
    }

    $installed = choco list -li
    $outdated = choco outdated

    $continue = $false;
    if($state) {
        $continue = $true;
    }

    $Config.packages | Get-Member -MemberType NoteProperty | ForEach-Object {
            $Name = $_.Name 

            if($continue) {
                if($name -eq $state) {
                    $continue = $false;
                }
                return;
            }

            $found = $false;
            foreach($line in $installed) {
                if($line -match $name) {
                    $found = $true;
                }
            }

            if($found) {
                $value = $Config.packages.$Name
                if($value.ToString() -eq "false") {
                    Write-Host "Chocolatey: Uninstall $Name"
                    Write-Host "----------------------------------------------------------"
                    choco uninstall $name -y
                    Write-Host ""
                    Write-Host ""
                    return;
                }
            }



            if(!$found) { 
                $value = $Config.packages.$Name
                
                
                

                Write-Host "Chocolatey: Install $Name"
                Write-Host "----------------------------------------------------------"
                if($value.ToString() -eq "true") {
                    choco install $name -y
                    Write-Host ""
                    Write-Host ""
                    return;
                }

                
                if($value -is [string]) {
                    Write-Debug $value;
                    choco install $name $value -y
                    Write-Host ""
                    Write-Host ""
                    return 
                }
                
                
                $argz = @()

                if($value.flags) {
                    $args += "-" + $value.flags 
                }

                if($value.version) {
                    $args += "--version"
                    $args += $value.version;
                }

                if($value.x86) {
                    $args += "--x86"
                }
                
                if($value.source) {
                    $argz+= "-s"
                    $argz+= "`"$($value.source)`""
                }

                if($value.ignoreChecksum) {
                    $argz += "--ignorechecksum"
                }

                if($value.pre) {
                    $argz += "--pre"
                }

                if($value.params) {
                    $data = "";
                    $value.params | Get-Member -MemberType NoteProperty | ForEach-Object {
                        $n = $_.Name 
                        $v = $_.Value;

                        if($v.ToString() -eq "true") {
                            $data += "/$n "
                            return;
                        }
                        if($v -is [string]) {
                            $v = "`"$v`""
                        }
                        $data += "/${n}:${v }"
                    }
                    $data = $data.Trim();

                    $argz += "--params=`"'$data'`""
                }

                if($value.installArgs) {
                    $data = "";
                    $value.installArgs | Get-Member -MemberType NoteProperty | ForEach-Object {
                        $n = $_.Name 
                        $v = $_.Value;

                        if($v.ToString() -eq "true") {
                            $data += "/$n "
                            return;
                        }
                        if($v -is [string]) {
                            $v = "`"$v`""
                        }
                        $data += "/${n}:${v }"
                    }
                    $data = $data.Trim();

                    $argz += "--ia=`"'$data'`""
                }

                Write-Debug $argz
                & $choco install $name @argz  

                if($value.restart) {
                    $dir = Split-Path $statePath
                    if(-not (Test-Path $dir)) {
                        New-Item $dir -ItemType Directory -Force | Write-Debug
                    } 
                    $name | Out-File $statePath -Encoding utf8
                    if($canDetectReboot) {
                        if(Test-PendingReboot) {
                            Invoke-Reboot
                        }
                    }
                    Write-Warning "A restart may be required."
                    exit;
                }

                return;
            }

           
            $i = 0;
            for(; $i -lt $outdated.Length; $i++) {
                $line = $outdated[$i];

                if($line -match "$Name\|") {
                    Write-Host "$Name is outdated"
                    if($line -match "\|true") {
                        break;
                    }

                    $value = $Config.packages.$Name
                    if($value -eq $false) {
                        Write-Host "Chocolatey: Remove $Name"
                        Write-Host "----------------------------------------------------------"
                        choco remove $name -y
                        Write-Host ""
                        Write-Host ""
                    }

                    if($value.ToString() -eq "true") {
                         
                        Write-Host "Chocolatey: Upgrade $Name"
                        Write-Host "----------------------------------------------------------"
                        choco upgrade $name -y
                        Write-Host ""
                        Write-Host ""
                        return 
                    }

                    if($value -is [string]) {
                        Write-Host "Chocolatey: Upgrade $Name $Value"
                        Write-Host "----------------------------------------------------------"
                        choco upgrade $name $value -y
                        Write-Host ""
                        Write-Host ""
                        return;
                    }




                    $argz = @()

                    if($value.flags) {
                        $args += "-" + $value.flags 
                    }

                    if($value.version) {
                        $args += "--version"
                        $args += $value.version;
                    }

                    if($value.x86) {
                        $args += "--x86"
                    }
                
                    if($value.source) {
                        $argz+= "-s"
                        $argz+= "`"$($value.source)`""
                    }

                    if($value.ignoreChecksum) {
                        $argz += "--ignorechecksum"
                    }

                    if($value.pre) {
                        $argz += "--pre"
                    }

                    if($value.params) {
                        $data = "";
                        $value.params | Get-Member -MemberType NoteProperty | ForEach-Object {
                            $n = $_.Name 
                            $v = $_.Value;

                            if($v.ToString() -eq "true") {
                                $data += "/$n "
                                return;
                            }
                            if($v -is [string]) {
                                $v = "`"$v`""
                            }
                            $data += "/${n}:${v }"
                        }
                        $data = $data.Trim();

                        $argz += "--params=`"'$data'`""
                    }

                    if($value.installArgs) {
                        $data = "";
                        $value.installArgs | Get-Member -MemberType NoteProperty | ForEach-Object {
                            $n = $_.Name 
                            $v = $_.Value;

                            if($v.ToString() -eq "true") {
                                $data += "/$n "
                                return;
                            }
                            if($v -is [string]) {
                                $v = "`"$v`""
                            }
                            $data += "/${n}:${v }"
                        }
                        $data = $data.Trim();

                        $argz += "--ia=`"'$data'`""
                    }

                    Write-Debug $argz
                    & $choco upgrade $name @argz  

                    if($value.restart) {
                        $dir = Split-Path $statePath
                        if(-not (Test-Path $dir)) {
                            New-Item $dir -ItemType Directory -Force | Write-Debug
                        } 
                        $name | Out-File $statePath -Encoding utf8
                        if($canDetectReboot) {
                            if(Test-PendingReboot) {
                                Invoke-Reboot
                            }

                            return;
                        }
                        Write-Warning "A restart may be required."
                        exit;
                    }
                }
            }
        }
    
}
