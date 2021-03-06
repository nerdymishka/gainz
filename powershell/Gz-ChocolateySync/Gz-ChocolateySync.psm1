
if(!$PSScriptRoot) {
    $PSScriptRoot = $MyInovocation.PSScriptRoot
}

if(!$PSScriptRoot) {
    $PSScriptRoot = Split-Path $MyInovocation.MyCommand.Path
}

Get-Item "$PsScriptRoot\private\*.ps1" | ForEach-Object {
    . "$($_.FullName)"
}


Get-Item "$PsScriptRoot\public\*.ps1" | ForEach-Object {
    . "$($_.FullName)"
}


Export-ModuleMember -Function @(
    'Expand-ChocolateyArchive',
    'Get-ChocolateyInstallLocation',
    'Get-ChocolateyTempDirectory',
    'Get-ChocolateyDecryptKey',
    'Get-ChocolateyTempInstallDirectory',
    'Get-WebRequestContentAsString',
    'Install-Chocolatey',
    'Install-BoxStarter',
    'Read-ChocolateyUpdateConfig',
    'Save-WebRequestContentAsFile',
    'Set-ChocolateyDecryptKey',
    'Sync-Chocolatey',
    'Update-ChocolateyPackages',
    'Update-ChocolateySources'
)