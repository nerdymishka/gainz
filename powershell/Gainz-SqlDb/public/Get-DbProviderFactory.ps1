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
        [String] $ProviderName = $null
    )

    PROCESS {
        if([string]::IsNullOrWhiteSpace($ProviderName)) {
            $ProviderName = "Default"
        }

        $factory = Get-SqlDbOption -Name "DbProviderFactories/$ProviderName"
        if($null -eq $factory) {
            if($ProviderName -eq "Default") {
                $instance = [System.Data.SqlClient.SqlClientFactory]::Instance
                Add-DbProviderFactory -Name "SqlServer" -Factory $instance -Default
                return $factory;
            }
            
            throw Exception "Could not locate factory for $ProviderName. Call Add-DbProviderFactory"
        }

        return $factory
    }
}