

function Write-ChocolatePackageParameters() {
    Param(
        [Parameter(Position = 0)]
        [PsCustomObject] $Parameters
    )

    $canDecrypt = ($null -ne (Get-Command Unprotect-String -ErrorAction SilentlyContinue))
    $decryptKey = Get-ChocolateyDecryptKey

    $paramStr = @();
    $Parameters | Get-Member -MemberType NoteProperty | ForEach-Object {
        $k = $_.Name;
        $v = $_.Value;

        $k = $k.ToUpper();

        if($null -eq $value) {
            $paramStr += "/$k";
            return ;
        }

        if($v -is [Boolean])
        {
            if($v) {
                $paramStr += "/$k"
            
            }
            return
        }

        if($v -is [String]) 
        {
            if($v.StartsWith("encrypted:")) {
                if(!$canDecrypt) {
                    Write-Error "Unprotect-String is not available in the session"
                    Write-Error "run: $ Install-Module Gainz-ProtectData -Force | Import-Module "
                    $msg = "$k is an ecrypted value. Unprotect-String is not availble"
                    $msg += " in the session. run: $ Install-Module Gainz-ProtectData -Force | Import-Module"
                    throw [Exception] 
                    return;
                }

                $v = $v.Substring(10)

                $v = Unprotect-String $v -PrivateKey $decryptKey
            }

            $v = "`"$v`""
        }

        $paramStr += "/${k}:${v}"
    }

    return $paramStr;
}


function Read-ChocolateyParameters() {
    Param(
        [Parameter(Position = 0)]
        [PsCustomObject] $Parameters
    )

     $p = $Parameters
    $switches = @();
    $params = @{};
    $splat = @{};
    $remove = $false;
    $flags = $null;

    $pp = @("pp", "parameters", "params")
    $ia =@("args", "ia", "installargs", "installarguments");
    $ov = @("o", "override", "overrideargs");
    $ig = @("iaglobal", "argsglobal", "installargsglobal");
    $pg = @("ppglobal", "paramaglobal", "parametersglobal"); 
    $m = @("m", "sxs");
    $n = @("n", "skipposh", "skippowershell");

    if($p.flags) {
        $flags = $p.flags
    }


    if($p.uninstall -or $p.remove) {
        $remove = $true;
    }

    $p = $Parameters;

    if($p.debug) {
        $switches += "debug";   
    }

    if($p.verbose) {
        $switches += "verbose";   
    }

    if($p.trace) {
        $switches += "trace";
    }

    if($p.force) {
        $switches += "force";
    }

    if($p.yes) {
        $switches += "yes";
    }

    if($p.allowunofficial -or $p.custom) {
        $switches += "allowunofficial"
    }

    
    
    if($p.checksum) {
        $c = $p.checksum;
        if($c.ignore) {
            $switches += "ignorechecksum"
        }
        if($c.allowEmpty) {
            $switches += "allowemptychecksum"
        }
        if($c.requireCheksum) {
            $switches += "requirechecksum"
        }
        if($c.hash) {
            $splat.Add("checksum", $c.hash)
        }

        if($c.hash64) {
            $splat.Add("checksum64", $c.hash64)
        }

        if($c.type) {
            $splat.Add("checksumtype", $c.type)
        }

        if($c.type64) {
            $splat.Add("checksumtype", $c.type64)
        }
    } 

    if($p.version) {
        $splat.Add("version", $p.version);
    }

    if($p.x86) {
        $splat["switches"] += "x86"
    }

    if($p.source -and ![string]::IsNullOrWhiteSpace($p.source)) {
        $splat.Add("source", "`"$($p.source)`"")
    }
    
   

    if($p.ignoreChecksum) {
       $switches += "ignorechecksum"
    }

    if($p.pre) {
        $switches += "pre"
    }

    $ppValues = $null;
    $iaValues = $null;
    $ovValues = $null;
    $gpValues = $null;
    $gaValues = $null;

    $p | Get-Member -MemberType NoteProperty | ForEach-Object {
        $k = $_.Name;
        $n = $_.Name.ToLower();

        if($pp.Contains($n)) {
            $ppValues = $p.$k;
            return;
        }

        if($ia.Contains($n)) {
            $iaValues = $p.$k;
            return;
        }

        if($ov.Contains($n)) {
            $ovValues = $p.$k;
            return;
        }

        if($pg.Contains($n)) {
            $pgValues = $p.$k;
            return;
        }

        #if($ga.Contains($n)) {
        #    $gaValues = $p.$k;
        #    return;
        #}
    }

    if($ppValues) {
       
        if($ppValues -is [string]) {
            if(![string]::IsNullOrWhiteSpace($ppValues)) {
                $params.Add("params", $ppValues);
            }
        } elseif($ppValues -is [PsCustomObject]) {
            $next = Write-ChocolatePackageParameters $ppValues
            $params.Add("params", $next);
        }
    }

    if($iaValues) {
       
        if($iaValues -is [string]) {
            if(![string]::IsNullOrWhiteSpace($iaValues)) {
                $params.Add("installargs", $iaValues);
            }
        } elseif($iaValues -is [PsCustomObject]) {
            $next = Write-ChocolatePackageParameters $iaValues
            $params.Add("installargs", $next);
        }
    }

    if($ovValues) {
       
        if($ovValues -is [string]) {
            if(![string]::IsNullOrWhiteSpace($ovValues)) {
                $params.Add("override", $ovValues);
            }
        } elseif($ovValues -is [PsCustomObject]) {
            $next = Write-ChocolatePackageParameters $ovValues
            $params.Add("override", $next);
        }
    }

    if($gpValues) {
       
        if($gpValues -is [string]) {
            if(![string]::IsNullOrWhiteSpace($gpValues)) {
                $params.Add("globalparams", $gpValues);
            }
        } elseif($gpValues -is [PsCustomObject]) {
            $next = Write-ChocolatePackageParameters $gpValues
            $params.Add("globalparams", $gpValues);
        }
    }

    if($gaValues) {
       
        if($gaValues -is [string]) {
            if(![string]::IsNullOrWhiteSpace($gaValues)) {
                $params.Add("globalargs", $gaValues);
            }
        } elseif($gaValues -is [PsCustomObject]) {
            $next = Write-ChocolatePackageParameters $gaValues
            $params.Add("gobalargs", $next);
        }
    }
   
    return @{switches = $switches; parameters = $params; splat = $splat; remove = $remove; flags = $flags; sleep = $p.sleep}
}


function Update-ChocolateyPackages() {
    Param(
        [Parameter(Position = 0)]
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
    $decryptKey = Get-ChocolateyDecryptKey
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


    if($config.packages -is [Array])
    {   
        $i = 0;
        foreach($item in $config.packages)
        {
            if($item -is [String])
            {
                $parts = $item.Trim().Split(' ')
                $name = $parts[0];

                $found = $false;

                $confirm = $item.contains("y") -or $item.contains("yes")
                if($confirm) {
                    $item += " --yes";
                }

                foreach($line in $installed) {
                    if($line -match $name) {
                        $found = $true;
                        break;
                    }
                }

                if($found)
                {
                    choco upgrade $item;

                    Write-Host ""
                    Write-Host ""
                    $i++
                    continue;
                }

                choco install $item;

                Write-Host ""
                Write-Host ""
                $i++;
                continue;
            }

            if($item -is [PsCustomObject])
            {
                $name = $item.name;
                if(!$name) {
                    Write-Warning "name is missing from chocolatey package item";
                    $i++;
                    continue;
                }

                $data = Read-ChocolateyParameters $item 
                $splat = @();

                if($data.splat.Count -gt 0) {
                    foreach($k in $data.splat.Keys) {
                        $splat += "--$k";
                        $splat += $data.splat[$k];
                    }
                }

                if($data.switches.Length -gt 0) {
                    foreach($k in $data.switches) {
                        $splat += "--$k";
                    }
                }
                
                if($data.parameters.Count -gt 0) {
                    foreach($k in $data.parameters.Keys) {
                        $v= $data.parameters[$k]
                        $splat += "--$k=`"'$v'`""
                    }
                }

                if($data.flags) {
                    $splat += "-" + $data.flags;
                }

                $found = $false;
                foreach($line in $installed) {
                    if($line -match $name) {
                        $found = $true;
                        break;
                    }
                }



                if($found)
                {
                   if($data.remove) {
                        Write-host ("choco remove " +  $name + " " + [string]::Join(" ", $splat)) 
                        & choco uninstall $name @splat
                        if($data.sleep) {
                            Start-Sleep ($data.sleep)
                        } 
                        Write-Host ""
                        Write-Host ""
                        $i++;
                        continue;
                   } 

                   $i = 0;
                   $isOutdated = $false
                   for(; $i -lt $outdated.Length; $i++) {
                        $line = $outdated[$i];

                        if($line -match "$Name\|") {
                            $isOutdated = $true;
                            break;
                        }
                    }
                    $isForced = $data.switches.Contains("force") -or ($data.flags -and $data.flags.Contains("f"));

                    if(!$isOutdated -and !$isForced) {
                        $i++
                        continue;
                    }

                    Write-host ("choco upgrade " +  $name + " " + [string]::Join(" ", $splat)) 
                    & choco upgrade $name @splat 
                    if($data.sleep) {
                    Write-host "sleeping"
                        Start-Sleep ($data.sleep)
                    }
                    Write-Host ""
                    Write-Host ""
                    $i++;
                    continue;
                }

                if(!$data.remove) {
                    Write-host ("choco install " +  $name + " " + [string]::Join(" ", $splat)) 
                    & choco install $name @splat
                    if($data.sleep) {
                        Start-Sleep ($data.sleep)
                    }
                    #& choco uninstall $name @splat 
                    Write-Host ""
                    Write-Host ""
                    $i++;
                    continue;
                }
            }
        }

        return;
    }
   

    $Config.packages.PsObject.Properties | ForEach-Object {
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
                    $failed = $false;
                    $value.params | Get-Member -MemberType NoteProperty | ForEach-Object {
                        $n = $_.Name 
                        $v = $_.Value;

                        

                        if($v.ToString() -eq "true") {
                            $data += "/$n "
                            return;
                        }
                        if($v -is [string]) {
                            if($v -and $v.StartsWith("encrypted:")) {
                              
                                if(!$canDecrypt) {
                                    $failed = $true
                                    Write-Warning "($Name)  Unprotect-String is not loaded. Run Install-Module/Import-Module Gainz-ProtectData -Force"
                                    return;
                                }
            
                                $v = Unprotect-String $v -PrivateKey $decryptKey;
                            }
                            $v = "`"$v`""
                        }
                        $data += "/${n}:${v }"
                    }

                    if($failed) {
                        return;
                    }
                    $data = $data.Trim();
                    

                    $argz += "--params=`"'$data'`""
                }

                if($value.installArgs) {
                    $data = "";
                    $failed = $false;
                    $value.installArgs | Get-Member -MemberType NoteProperty | ForEach-Object {
                        $n = $_.Name 
                        $v = $_.Value;

                        if($v.ToString() -eq "true") {
                            $data += "/$n "
                            return;
                        }
                        if($v -is [string]) {
                            if($v -and $v.StartsWith("encrypted:")) {
                              
                                if(!$canDecrypt) {
                                    $failed = $true
                                    Write-Warning "($Name)  Unprotect-String is not loaded. Run Install-Module/Import-Module Gainz-ProtectData -Force"
                                    return;
                                }
            
                                $v = Unprotect-String $v -PrivateKey $decryptKey;
                            }
                            $v = "`"$v`""
                        }
                        $data += "/${n}:${v }"
                    }
                    if($failed) {
                        return;
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
                        $failed = $false;
                        $value.params | Get-Member -MemberType NoteProperty | ForEach-Object {
                            $n = $_.Name 
                            $v = $_.Value;

                            if($v.ToString() -eq "true") {
                                $data += "/$n "
                                return;
                            }
                            if($v -is [string]) {
                                if($v -and $v.StartsWith("encrypted:")) {
                              
                                    if(!$canDecrypt) {
                                        $failed = $true
                                        Write-Warning "($Name)  Unprotect-String is not loaded. Run Install-Module/Import-Module Gainz-ProtectData -Force"
                                        return;
                                    }
                
                                    $v = Unprotect-String $v -PrivateKey $decryptKey;
                                }
                                $v = "`"$v`""
                            }
                            $data += "/${n}:${v }"
                        }
                        if($failed) {
                            return;
                        }
                        $data = $data.Trim();

                        $argz += "--params=`"'$data'`""
                    }

                    if($value.installArgs) {
                        $data = "";
                        $failed = false;
                        $value.installArgs | Get-Member -MemberType NoteProperty | ForEach-Object {
                            $n = $_.Name 
                            $v = $_.Value;

                            if($v.ToString() -eq "true") {
                                $data += "/$n "
                                return;
                            }
                            if($v -is [string]) {
                                if($v -and $v.StartsWith("encrypted:")) {
                              
                                    if(!$canDecrypt) {
                                        $failed = $true
                                        Write-Warning "($Name)  Unprotect-String is not loaded. Run Install-Module/Import-Module Gainz-ProtectData -Force"
                                        return;
                                    }
                
                                    $v = Unprotect-String $v -PrivateKey $decryptKey;
                                }
                                if($failed) {
                                    return;
                                }
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
