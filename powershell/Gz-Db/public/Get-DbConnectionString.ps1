
function Get-DbConnectionString() {
    <#
        .SYNOPSIS
        Gets the default global connection string
    
        .DESCRIPTION
        This function is called in absense of a specified connection string 
        for functions that require them.  
    
        .PARAMETER Name
        Optional.  The name of the connection string to retrieve. If the name is not specified,
        the name defaults to 'Default'.  
    
        .EXAMPLE
        $connectionString = Get-DbConnectionString
    
        .EXAMPLE
        $connectionString = Get-DbConnectionString -Name "MySql"
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
            $connectionStrings = Get-DbOption -Name "ConnectionStrings"
            $keys = $connectionStrings.Keys 
            $providerNameAttrs.Add((New-Object  System.Management.Automation.ValidateSetAttribute($keys)));
            $providerName = New-Object System.Management.Automation.RuntimeDefinedParameter("Name", [String], $providerNameAttrs)        
            $runtimeParameters.Add("Name", $providerName)            
    
            return $runtimeParameters;
        }
    
        Process {
            $Name = $PSBoundParameters["Name"];
            if(!$Name) { $Name = "Default" }
            return Get-DbOption -Name "ConnectionStrings/$Name" 
        }
    }