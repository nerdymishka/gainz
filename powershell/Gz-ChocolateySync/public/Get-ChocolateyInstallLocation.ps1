function Get-ChocolateyInstallLocation() {

    $chocolateyInstall = $Env:ChocolateyInstall
    if([string]::IsNullOrWhiteSpace($chocolateyInstall)) {
        $chocolateyInstall = Join-Path $env:ALLUSERSPROFILE "Chocolatey" 
    }

    if(-not (Test-Path $chocolateyInstall)) {
        $chocolateyInstall = Join-Path $env:SystemDrive "ProgramData" 
        $chocolateyInstall = Join-Path $chocolateyInstall "Chocolatey"
    }

    return $chocolateyInstall
}