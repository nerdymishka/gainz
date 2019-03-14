function Invoke-ChocolateySetupScript() {

    # Call chocolatey install
    Write-Debug "Installing chocolatey on this machine"
    $tmpDir = Get-ChocolateyTempInstallDirectory
    $toolsFolder = Join-Path $tmpDir "tools"
    $chocInstallPS1 = Join-Path $toolsFolder "chocolateyInstall.ps1"

    & $chocInstallPS1
}