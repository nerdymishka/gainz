


if(!$PSScriptRoot) {
    $PSScriptRoot = $MyInovocation.PSScriptRoot
}

if(!$PSScriptRoot) {
    $PSScriptRoot = Split-Path $MyInovocation.MyCommand.Path
}

Get-Item "$PsScriptRoot\private\*.ps1" -EA SilentlyContinue | ForEach-Object {
    . "$($_.FullName)"
}


Get-Item "$PsScriptRoot\public\*.ps1" | ForEach-Object {
    . "$($_.FullName)"
}

Export-ModuleMember -Function @(
    'Add-DynamicParameter',
    'ConvertTo-Hashtable',
    'New-DynamicParameter',
    'Out-FileNoBom',
    'Out-Json',
    'Read-GzModuleSetting',
    'Set-XFileMode',
    'Set-XFileOwner',
    'Test-64Bit',
    'Test-OsPlatform',
    'Test-UserIsAdministrator',
    'Write-GzModuleSetting'
)