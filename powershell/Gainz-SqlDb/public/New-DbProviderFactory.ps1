function New-DbProviderFactory() {
    <#
        .SYNOPSIS
        Creates a new instance of DbProviderFactory by name.
    
        .DESCRIPTION
        Internally uses `System.Data.Common.DbProviderFactories` to create
        a new DbProviderFactory instance.
    
        .PARAMETER ProviderName
        The of the Database Provider Factory such as 
        "System.Data.SqlClient", "MySql.Data.MySqlClient", "Npgsql2 Data Provider"
    
        .EXAMPLE
        $factory = New-DbProviderFactory "System.Data.SqlClient"
    
    #>
        [CmdletBinding()]
        Param(
           [String] $Provider
        )


       


        PROCESS {
            

            return [System.Data.Common.DbProviderFactories]::GetFactory($Provider);
        }
    }