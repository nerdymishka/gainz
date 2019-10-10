
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

# Set-GzDbPRoviderFactoryDefault 


Export-ModuleMember -Function  @(
    'Add-DbProviderFactory',
    'Get-DbOption',
    'Set-DbOption',
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
    'Invoke-DbCommand'
)