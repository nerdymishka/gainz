function Read-ChocolateyUpdateConfig() {
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $true)]
        [String] $Uri,

        [switch] $Decrypt,

        [byte[]] $DecryptKey
    )

    $configDir = "$HOME/.config/nerdymishka/chocolatey"
    if([string]::IsNullOrWhiteSpace($Uri)) {
        $Uri = $Env:ChocolateyUpdateConfig
        if(!$Uri) {
            $Uri = $Env:CHOCOLATEY_UPDATE_CONFIG
        }
    }

    if(!$Uri) {
        if(Test-Path "$configDir/config.json") {
            $Uri = "$configDir/config.json"
        }
    }

    if(!$Uri) {
        Write-Warning "Uri for the configuration must be set";
        return;
    }

    if($Uri.StartsWith("./")) {
        $Uri = (Resolve-Path $Uri).Path
    }

    $test = [Uri]$Uri
    if(!$test.IsFile) {
        $content = Get-WebRequestContentAsString -Uri $Uri
    } else {
        $content = Get-Content $Uri -Raw
    }

    if($DecryptKey -or $Decrypt.ToBool()) {
        $canDecrypt = $null -ne (Get-Command Unprotect-String -ErrorAction SilentlyContinue)
        if(!$canDecrypt) {
            Write-Warning "Unprotect-String is required to decrypt data"
            return;
        }
        if(!$DecryptKey) {
            $DecryptKey = Get-ChocolateyDecryptKey
            if(!$DecryptKey) {
                Write-Warning "DecryptKey is required to decrypt data."
                return;
            }
        }

        $content = Unprotect-String -PrivateKey $DecryptKey -EncryptedValue $content
    }

    return $content | ConvertFrom-Json 
}