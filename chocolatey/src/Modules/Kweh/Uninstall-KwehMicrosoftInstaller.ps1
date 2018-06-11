function Uninstall-KwehMicrosoftInstaller() {
    Param(
        [String] $DisplayName,
        [String] $Guid,
        [String] $SilentArgs = "/qn /norestart"
    )

    if([string]::IsNullOrWhiteSpace($Guid) -and [string]::IsNullOrWhiteSpace($DisplayName)) {
        throw ArgumentException "DisplayName or Guid must be specified"
    }

    if(![string]::IsNullOrWhiteSpace($Guid)) {
        msiexec.exe /x $Guid $SilentArgs
        Start-Sleep 1
        return $LASTEXITCODE 
    }

    $key = Find-RegistryUninstallKey -DisplayName $DisplayName
    if($key) {
        Write-Error "Could not locate unins9tall key for $DisplayName"
        return $null;
    }

    $uninstallString = $key.GetValue("UninstallString")
    if($uninstallString -and $uninstallString.ToLower().StartsWith("msi")) {
        $leftBracket = $uninstallString.IndexOf($uninstallString)
        $guidString = $uninstallString.Substring($leftBracket  + 1, 32)
        [guid] $guid 
        if([System.Guid]::TryParse($guidString, [out] $guid)) {
            $guidString = "{$guidString}"
            msiexec.exe /x $guidString $SilentArgs
            Start-Sleep 1
            return $LASTEXITCODE
        } else {
            Write-Error "Could not parse guid from $uninstallString" 
        }
    } else {
        Write-Error "Uninstall Key not found for $key"
    }
    
    return $null;
}

Set-Alias -Name Uninstall-MicrosoftInstaller -Value Uninstall-KwehInnoInstaller