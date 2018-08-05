
$chocolateyPrivateKey = $null;

function Set-ChocolatelyDecryptKey() {
    Param(
        [Parameter(Position = 0,ValueFromPipeline = $true)]
        [Byte[]] $Key
    )

    $chocolateyPrivateKey = $Key
}

function Get-ChocolatelyDecryptKey() {
    if($chocolateyPrivateKey) {
        return $chocolateyPrivateKey
    }

    if($Env:CHOCOLATEY_DECRYPT_KEY) {
        switch -Regex ($Env:CHOCOLATEY_DECRYPT_KEY) {
            "^file:" {  
                $file = $Env:CHOCOLATEY_DECRYPT_KEY
                $file = $file.Replace("file://", "")
                $chocolateyPrivateKey = [System.IO.File]::ReadAllBytes($file);
            }
            "^secureString:" {
                $name = $Env:CHOCOLATEY_DECRYPT_KEY
                $name = $name.Replace("secureString://", "")
                $value = Get-Item Variable:\$name
                $chocolateyPrivateKey = ConvertTo-UnprotectedBytes -SecureString $value  
            }
            "^thumprint:" {
                Write-Warning "Not supported yet"
            }
            Default {
                $chocolateyPrivateKey = [System.Text.Encoding]::UTF8.GetBytes($Env:CHOCOLATEY_DECRYPT_KEY)
            }
        }

        return $chocolateyPrivateKey;
    }

    return $false;
}

