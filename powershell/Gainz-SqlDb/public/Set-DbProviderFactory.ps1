function Set-DbProviderFactory() {
    <#
        .SYNOPSIS
        Sets the default global provider factory.
    
        .DESCRIPTION
        The default global provider factory is used by the other functions / 
        cmdlets in the this module to construct Connection, Commands, Transaction
        and Parameter objects when a provider factory is not specified.
    
        .PARAMETER Factory
        An instance of `System.Data.Common.DbProviderFactory`
    
        .EXAMPLE
        Set-DbProviderFactory ([MySql.Data.MySqlClient.MySqlClientFactory]::Instance)
    
    #>
        Param(
            [Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true)]
            [System.Data.Common.DbProviderFactory] $Factory
        )

        Set-DbSqlOption -Name "DbFactory" -Value $Factory 
    }