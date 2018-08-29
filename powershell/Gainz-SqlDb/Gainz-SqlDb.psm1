
if(!$PSScriptRoot) {
    $PSScriptRoot = $MyInovocation.PSScriptRoot
}

if(!$PSScriptRoot) {
    $PSScriptRoot = Split-Path $MyInovocation.MyCommand.Path
}

Get-Item "$PsScriptRoot\public\*.ps1" | ForEach-Object {
     . "$($_.FullName)"
}

$functions  = @(
    'Add-DbProviderFactory',
    'Get-SqlDbOption',
    'Set-SqlDbOptions',
    'Set-DbConnectionString',
    'Get-DbConnectionString',
    'New-DbProviderFactory',
    'Set-DbProviderFactory',
    'Get-DbProviderFactory',
    'Get-DbParameterPrefix',
    'Set-DbParameterPrefix',
    'New-DbConnection',
    'New-DbCommand',
    'Read-DbData',
    'Write-DbData',
    'Invoke-DbCmd'
)
foreach($func in $functions) {
    Export-ModuleMember -Function $func
}
