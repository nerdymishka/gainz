if(!$PSScriptRoot) {
    $PSScriptRoot = $MyInovocation.PSScriptRoot
}

if(!$PSScriptRoot) {
    $PSScriptRoot = Split-Path $MyInovocation.MyCommand.Path
}

Get-Item "$PSScriptRoot\public\*.ps1" | ForEach-Object {
    . "$($_.FullName)"
}

#Get-Item "$PSScriptRoot\private\*.ps1" | ForEach-Object {
#    . "$($_.FullName)"
#}

Export-ModuleMember -Function @(
    'Get-GainzModuleOption',
    'New-DynamicParameter',
    'Read-GainzModuleOption',
    'Set-GainzModuleOption',
    'Set-NonInteractive',
    'Test-Interactive',
    'Test-IsElevated',
    'Write-Banner',
    'Write-GainzModuleOption'
)