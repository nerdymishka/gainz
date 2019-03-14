
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
    'Get-GzWinAutopilotInfo',
    'Get-GzWinCloudJoinUser',
    'Get-GzWinLocalGroupMember',
    'Invoke-GzWinTelemetry',
    'Merge-GzWinTelemetryObject',
    'Read-GzPowershellModule',
    'Read-GzWin32App',
    'Read-GzWinAdministratorMember',
    'Read-GzWinAppXPackage',
    'Read-GzWinBitLockerStatus',
    'Read-GzWinChromeExtension',
    'Read-GzWinEnabledFeature',
    'Read-GzWinLocalGroup',
    'Read-GzWinLocalUser',
    'Read-GzWinScheduledTask',
    'Read-GzWinService',
    'Read-GzWinUserFile',
    'Read-GzWinVolume',
    'Register-GzRegistryUserHive'
)