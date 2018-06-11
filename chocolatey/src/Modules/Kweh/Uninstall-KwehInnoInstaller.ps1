function Uninstall-KwehInnoInstaller() {
    Param(
        [String] $DisplayName,
        [String] $Path,
        [String] $Log,
        [String] $SilentArgs = "/VERYSILENT /SUPPRESSMSGBOXES /NORESTART"
    )

    if(![string]::IsNullOrWhiteSpace($Log)) {
        $SilentArgs += " /Log=`"$Log`""
    }

    $key = Find-RegistryUninstallKey -DisplayName $DisplayName
    if($key) {
        Write-Error "Could not locate uninstall key for $DisplayName"
        return $null;
    }

    $uninstallString = $key.GetValue("UninstallString")
    if($uninstallString) {
        $uninstallString = $uninstallString.Trim();
        & $uninstallString $SilentArgs
    }

    if(! (Test-Path $Path)) {
        throw System.IO.FileNotFoundException $Path 
    } 

    & $Path $SilentArgs
}

Set-Alias -Name Uninstall-InnoInstaller -Value Uninstall-KwehInnoInstaller 