if((Get-Command New-KeePassKey -ErrorAction SilentlyContinue) -eq $null) {
    . "$PSScriptRoot\New-KeePassKey.ps1"
}

function Find-KeePassEntryByTitle() {
<#
    .SYNOPSIS 
    Searches for the first entry in the KeePass database with
    a match on the given title

    .DESCRIPTION
    This method does a case insensivive equality search against the titles of
    all the entries and returns the first match

    .PARAMETER Package
    The object representation of the KeePass database xml file created by
    the `Open-KeePassPackage` function 

    .PARAMETER Title 
    The title for the entry.

    .EXAMPLE
    $entry = $Package | Find-KeePassEntryByTitle "cert:azure"

#>
    Param(
        [Parameter(Mandatory = $true, Position = 1, ValueFromPipeline = $true)]
        [NerdyMishka.KeePass.KeePassPackage] $Package,
        
        [Parameter(Mandatory = $true, Position = 0)]
        [string] $Title 
    )

 
    return $Package.FindEntriesByTitle($Title)
}