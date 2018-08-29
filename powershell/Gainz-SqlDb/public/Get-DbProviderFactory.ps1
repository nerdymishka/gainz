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
        
    )

    PROCESS {
        $factory = Get-SqlDbOption -Name "DbFactory"
        if(!$factory) {
            $factory = New-DbProviderFactory -ProviderName "SqlServer"
            Set-SqlDbOption -Name "DbFactory" -Value $factory;
        }
    
        return $factory;
    }
}