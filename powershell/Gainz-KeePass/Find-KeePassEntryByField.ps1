

function Find-KeePassEntryByField() {
<#
    .SYNOPSIS 
    Searches for the all the entries in the KeePass database with
    that match the given field.

    .DESCRIPTION
    This method allows for comparisons against the Title, UserName, Url,
    and Tags field of each entry in the Database. 

    It will return all the matches that it finds.  

    .PARAMETER Package
    The object representation of the KeePass database xml file created by
    the `Open-KeePassPackage` function 

    .PARAMETER Name
    The name for the field to search against.

    .PARAMETER Value
    The value to use for the comparison

    .PARAMETER CaseInsenstive
    If present, the search will be case insensitive.

    .PARAMETER Comparison
    The type of string comparison that will be used: Equal, StartsWith, EndsWith, Contains

    .EXAMPLE
    $entries = $Package | Find-KeePassEntryByField "Url" "http://google.com"

#>
    Param(
        [Parameter(Mandatory = $true, Position = 2, ValueFromPipeline = $true)]
        [NerdyMishka.KeePass.KeePassPackage] $Package,
        
        [Parameter(Mandatory = $true, Position = 0)]
        [string] $Name,
        
        [Parameter(Mandatory = $true, Position = 1)]
        [string] $Value,
        
        [switch] $CaseInsensitive,
        
        [NerdyMishka.KeePass.StringValueComparison] $Comparison  
    )

 
    return $Package.FindEntriesByField($Name, $Value, $CaseInsenstive.ToBool(), $Comparison)
}