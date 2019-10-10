

function Add-DbProviderFactory() {
<#
.SYNOPSIS
    Add a db provider factory.
.DESCRIPTION
    Add a db provider factory by name and factory instance.
.EXAMPLE
    PS C:\> Add-DbProviderFactory -Name "MySql" -Factory [MySQL.Data.MySQLClient.MySqlClientFactory]::Instance
    
.INPUTS
    Inputs (if any)
.OUTPUTS
    None
.NOTES
    General notes
#>
    Param(
        [Parameter(Position = 0, Mandatory = $true)]
        [String] $Name,

        [Parameter(Position = 1, Mandatory = $true, ValueFromPipeline = $true)]
        [System.Data.Common.DbProviderFactory] $Factory,

        [Switch] $Default 
    )

    Set-DbOption -Name "DbProviderFactories/$Name" -Value $Factory

    if($Default.ToBool()) {
        Set-DbOption -Name "DbProviderFactories/Default" -Value $Factory
    }
}