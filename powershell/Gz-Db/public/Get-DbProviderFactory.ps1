function Get-DbProviderFactory() {
    <#
        .SYNOPSIS
        Gets the default SqlProviderFactory
        .DESCRIPTION
        The default global provider factory is used by the other functions / 
        cmdlets in the this module to construct Connection, Commands, Transaction
        and Parameter objects when a provider factory is not specified.
    
        .PARAMETER ProviderName
        An instance of `System.Data.Common.DbProviderFactory`
    
        .EXAMPLE
        $factory = Get-DbProviderFactory
    
        .EXAMPLE
        $factory = Get-DbProviderFactory 
    
    #>
    [CmdletBinding()]
    Param(
        [Parameter(Position = 0)]
        [String] $ProviderName = "Default"
    )

    PROCESS {
        

        $factory = Get-DbOption -Name "DbProviderFactories/$ProviderName"
        if($null -eq $factory) {
            if($ProviderName -eq "Default") {
                $instance = [System.Data.SqlClient.SqlClientFactory]::Instance
                Add-DbProviderFactory -Name "SqlServer" -Factory $instance -Default

                $instance = [Microsoft.Data.Sqlite.SqliteFactory]::Instance
                if($instance -eq $null) { throw "sqlite factory is null"}
                Add-DbProviderFactory -Name "Sqlite" -Factory $instance
               
                return $factory;
            }
            
            throw "Could not locate factory for $ProviderName. Call Add-DbProviderFactory"
        }

        return $factory
    }
}