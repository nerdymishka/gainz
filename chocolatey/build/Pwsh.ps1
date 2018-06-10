<#
    Runs after Profile.ps1 is loaded by PowerShell Core.
#>

$cwd = (Resolve-Path "$PsScriptRoot/..").Path.Replace("\", "/")
$drive = "$cwd/test"

$env:ChocolateyInstall = "$drive/opt/chocolatey"
$env:ChocolateyToolsLocation = "$drive/opt"
$env:ChocolateyIgnoreProxy = $true

if(!(Test-Path $env:ChocolateyInstall)) {
    & "$PSScriptRoot/resources/Install-Chocolatey.ps1"
}