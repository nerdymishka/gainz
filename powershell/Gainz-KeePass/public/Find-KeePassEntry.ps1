

function Find-KeePassEntry() {
    <#
        .SYNOPSIS 
        Finds an entry based upon a path to the entry using group names
        and entry title.
    
        .DESCRIPTION
        This cmdlet will return null if a group or entry in the path
        is not found, other wise it will transverse the KeePass
        tree to find the entry by path. 
    
        .PARAMETER Package
        The object representation of the KeePass database xml file created by
        the `Open-KeePassPackage` function 
    
        .PARAMETER Path
        The path to the entry where the title is name of the entry.
    
        .PARAMETER CaseInsenstive
        If present, the search will be case insensitive.
    
    
        .EXAMPLE
        $entries = $Package | Find-KeePassEntry "PackageName/SubGroup/EntryTitle"
    
    #>
        Param(
            [Parameter(Mandatory = $true, Position = 2, ValueFromPipeline = $true)]
            [NerdyMishka.KeePass.IKeePassPackage] $Package,
            
            [Parameter(Mandatory = $true, Position = 0)]
            [string] $Path,
            
            [switch] $CaseInsenstive 
        )
    
     
        return $Package.FindEntry($Path, $CaseInsenstive.ToBool())
    }