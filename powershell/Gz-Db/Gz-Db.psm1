
if(!$PSScriptRoot) {
    $PSScriptRoot = $MyInovocation.PSScriptRoot
}

if(!$PSScriptRoot) {
    $PSScriptRoot = Split-Path $MyInovocation.MyCommand.Path
}



Get-Item "$PsScriptRoot\public\*.ps1" | ForEach-Object {
     . "$($_.FullName)"
}

# Load default providers. 
$instance = [System.Data.SqlClient.SqlClientFactory]::Instance
Add-GzDbProviderFactory -Name "SqlServer" -Factory $instance -Default

$instance = [System.Data.Sqlite.SqliteFactory]::Instance
if(!$instance) { throw "sqlite factory is null"}
Add-GzDbProviderFactory -Name "Sqlite" -Factory $instance


Export-ModuleMember -Function  @(
    'Add-GzDbAlias',
    'Add-GzDbProviderFactory',
    'Get-GzDbOption',
    'Set-GzDbOption',
    'Set-GzDbConnectionString',
    'Get-GzDbConnectionString',
    'New-GzDbProviderFactory',
    'Set-GzDbProviderFactory',
    'Get-GzDbProviderFactory',
    'Get-GzDbParameterPrefix',
    'Set-GzDbParameterPrefix',
    'New-GzDbConnection',
    'New-GzDbCommand',
    'Read-GzDbData',
    'Write-GzDbData',
    'Invoke-GzDbCommand',
    'Remove-GzDbAlias'
)