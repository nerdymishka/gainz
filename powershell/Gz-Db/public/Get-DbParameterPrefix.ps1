function Get-DbParameterPrefix() {
    <#
        .SYNOPSIS
        Gets the default sql parameter prefix such as '@'
    
        .DESCRIPTION
        This function is called in absense of a specified parameter prefix
        for many of the functions / cmdlets in this modules.
    
        .EXAMPLE
        $parameterPrefix = Get-DbParameterPrefix
    #>
    [CmdletBinding()]
    Param(

    )

    Process {
        $parameterPrefix = Get-DbOption -Name "ParameterPrefix"
        if(!$parameterPrefix) {
            return "@";
        }
        
        return $parameterPrefix;
    }
 }