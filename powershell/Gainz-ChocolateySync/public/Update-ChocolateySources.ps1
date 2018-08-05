function Update-ChocolateySources() {
    Param(
        [PsCustomObject] $Config
    )

    Write-Banner 

    <#
{
    install: "C:/apps"
    sources: {
        "feed1": "string",
        "feed2": {
            uri: "string"
            encrypted: true
        },
        "feed3": {
            uri: "string"
            user: true
            password: true
        }
        "feed4": { remove: true}
    }
}
#>

    $choco = Get-Command choco -ErrorAction SilentlyContinue
    if(!$choco) {
        return;
    }
    $choco = $choco.Path;

    if($Config.sources) {

        $sources = & $choco source
        $Config.sources | Get-Member -MemberType NoteProperty | ForEach-Object {
            $name = $_.Name 
            $found = $false;
            $update = $false;

            $set = $Config.sources.$name
            $feed = $set;
            $user = $null;
            $password = $null;
            $remove = $false;
            
            if(! $feed -is [string]) {
                $feed = $set.uri
                $remove = $set.remove;
                if($set.user) {
                    $user = $set.user;
                }

                if($set.password) {
                    $password = $set.password;
                }

                if($set.encrypted) {
                    $canEncrypt = $null -ne (Get-Command Unprotect-String -ErrorAction SilentlyContinue)
                    if(!$canEncrypt) {
                        Write-Warning "Skip source $($name): Unprotected-String is missing"
                        continue;
                    }
                    $decryptKey = Get-ChocolateyDecryptKey
                    if(!$decryptKey) {
                        Write-Warning "Skip source $($name): The decrypt key required to decrypt values is missing";
                        continue;
                    }
                    $feed = Unprotect-String -PrivateKey $decryptKey -EncryptedValue $feed
                    if($set.password) {
                        $password = Unprotect-String -PrivateKey $decryptKey -EncryptedValue ($set.password)
                    }
                }
            }

            foreach($line in $sources) {
                if($line -match $name) {
                    $found = $true;
                    
                    if($line -match $feed) {
                        break;
                    }

                    $update = $true;
                }
            }

            if($found -and $remove) {
                Write-Host "Chocolatey: Remove Source $name"
                Write-Host "----------------------------------------------------------"
                choco source remove -n="$name"
                Write-Host ""
                return;      
            }

            if($found -and !$update) {
                return;
            }

            
            if(!$found) {
                Write-Host "Chocolatey: Add Source $name"
                Write-Host "----------------------------------------------------------"
            }
            if($update) {
                Write-Host "Chocolatey: Update Source $name"
                Write-Host "----------------------------------------------------------"
                choco source remove -n="$name"     
            }
            
            if($user) {
                choco source add -n="$name" -s="$value" -u=$user -p="$password"
            } else {
                choco source add -n="$name" -s="$value"
            }
            
            Write-Host ""
        }
    }
}