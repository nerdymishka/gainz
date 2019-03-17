
$vsVersions = @{
    "10" = "10.0"
    "10.0" = "10.0"
    "2010" = "10.0"
    "Visual Studio 2010" = "10.0"

    "11" = "11.0"
    "11.0" = "11.0"
    "2012" = "11.0"
    "Visual Studio 2012" = "12.0"

    "12" = "12.0"
    "12.0" = "12.0"
    "2013" = "12.0"
    "Visual Studio 2013" = "12.0"
    
    "14" = "14.0"
    "14.0" = "14.0"
    "2015" = "14.0"
    "Visual Studio 2015" = "14.0"

    "15" = "15.0"
    "15.0" = "15.0"
    "2017" = "15.0"
    "Visual Studio 2017" = "15.0"
}

Set-ModuleVariable -Name "VsVersions" -Value $vsVersions
function Get-GzVisualStudioVersion() {
 
<#
.SYNOPSIS
    Get a visual studio version for the specified alias

.DESCRIPTION
    Visual studio has multiple labels for numeric versions
    such as Visual Studio 2017, 2017, 15, etc. The actual
    version number is a decimal such as 14.0, 15.0.

.PARAMETER INPUTOBJECT
    The label such as Visual Studio 2017 which returns 15.0 

.EXAMPLE
    PS C:\> Get-GzVisualStudioVersion -Alias "Visual Studio 2017"
    returns 15.0

.INPUTS
    The visual studio label/alias
.OUTPUTS
    Returns the numeric Visual Studio Version
    
#>
    [CmdletBinding()]
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $True)]
        [Alias("Label")]
        [Alias("Alias")]
        [string] $InputObject
    )

    if([string]::IsNullOrWhiteSpace($InputObject) -or $InputObject -eq "latest") {
        $latest = Get-ModuleVariable -Name "VsLatestVersion"
        if($null -ne $latest) {
            return $latest;
        }

        $vsPaths = Get-GzVisualStudioPath 
        $latest =  $vsPaths.Name | Sort-Object -Descending | Select-Object -First 1 
        Set-ModuleVariable -Name "VsLatestVersion" -Value $latest
        
        return $latest 
    }

    $vsVersions = Get-ModuleVariable -Name "VsVersions"

    if($vsVersions.ContainsKey($InputObject)) {
        return $vsVersions[$InputObject];
    }
    
    Throw "Unknown Visual Studio Version Label $InputObject"
}