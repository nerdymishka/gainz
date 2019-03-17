function Set-GzDbProviderFactoryDefault() {
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
        Set-GzDbProviderFactory "Sqlite"
    
    #>
    Param(
        [Alias("pn")]
        [Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true)]
        [String] $ProviderName 
    )

    $factory = Get-GzDbOption -Name "DbProviderFactories/$ProviderName"
    if(!$factory) {
        throw "Could not locate factory $ProviderName"
    }

    Set-GzDbOption -Name "DbProviderFactories/Default" -Value $factory
}