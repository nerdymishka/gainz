
function Set-DbConnectionString() {
<#
    .SYNOPSIS
    Sets the default global connection string and optionally the
    provider used to create the DbProviderFactory.

    .DESCRIPTION
    An alternate ConvertTo-Json method that outputs readable json unlike
    the native version for Powershell 5 and below. 

    .PARAMETER ConnectionString
    The string of key pair values that is used to construct a connection 
    to a resource such as a database server.

    .PARAMETER Name
    (Optional) The of the Database Provider Factory such as 
    "System.Data.SqlClient", "MySql.Data.MySqlClient", "Npgsql2 Data Provider"

    .EXAMPLE
    Set-DbConnectionString "Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True"

#>
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [String] $ConnectionString,
        
        [String] $Name = "Default"
    )

    Process {
        Set-SqlDbOption -Name "ConnectionStrings/$Name" -Value $ConnectionString 
    }
}



