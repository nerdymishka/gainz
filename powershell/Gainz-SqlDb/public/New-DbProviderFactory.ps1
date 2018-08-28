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
           
        )


        DynamicParam {
            $runtimeParameters  = New-Object System.Management.Automation.RuntimeDefinedParameterDictionary
            $providerNameAttr  = New-Object System.Management.Automation.ParameterAttribute
            $providerNameAttr.Mandatory  = $false 
            $providerNameAttr.Position = 0
            $providerNameAttr.ParameterSetName  = '__AllParameterSets'
            $providerNameAttrs = New-Object  System.Collections.ObjectModel.Collection[System.Attribute]
            $providerNameAttrs.Add($providerNameAttr)
            $factories = Get-SqlDbOption -Name "DbProviderFactories"
            $keys = $factories.Keys 
            $providerNameAttrs.Add((New-Object  System.Management.Automation.ValidateSetAttribute($keys)));
            $runtimeParameters.Add("ProviderName", $providerNameAttrs)            
  
            return $runtimeParameters;
        }



        PROCESS {
            $ProviderName = $PSBoundParameters["ProviderName"] 
            if(!$ProviderName) { $ProviderName = "SqlServer" }

            $factories = Get-SqlDbOption -Name "DbProviderFactories"
            if(! ($factories -is [Hashtable])) {
                Write-Error "Get-SqlDbOption is not returning a hashtable for DbProviderFactories. Reload the module";
                return $null;
            }

            if(!$factories.ContainsKey($ProviderName)) {
                Write-Error "DbProviderFactories does not contain a key for for $ProviderName";
                return $null;
            }

            $factory = $factories[$ProviderName];

            return [System.Data.Common.DbProviderFactories]::GetFactory($factory);
        }
    }