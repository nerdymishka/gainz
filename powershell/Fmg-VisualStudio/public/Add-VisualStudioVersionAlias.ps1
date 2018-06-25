function Add-VisualStudioVersionAlias() {
    <# 
        .SYNOPSIS 
        Adds a Visual Studio Version Mapping. e.g -Alias "Visual Studio 2018" -Version "16.0"
    
        .DESCRIPTION
        Mappings are used to create aliases for the version number of Visual Studio which
        are used to retrieve the install path for a specific version of Visual Studio
    
        .PARAMETER Alias
        One or more aliases used for a version of Visual Studio
    
        .PARAMETER Version
        The version number, which is the Major and Minor version numbers, for Visual Studio e.g. 
        version 15 is 15.0
    
        .EXAMPLE
        Add-VisualStudioVersionMapping -Alias "Visual Studio 2018","2018", "16" -Version "16.0"
    #>
        Param(
            [Paramter(Mandatory = $true, Position = 0)]
            [string[]] $Alias,
            [Parameter(Mandatory = $true, Position = 1)]
            [string] $Version  
         )
    
         if([string]::IsNullOrWhiteSpace($Version)) {
             throw [ArgumentNullException]("Version")
         }
    
         if(!$Version.Contains(".")) {
             throw [ArgumentException]("Version MUST have a major and minor version")
         }
    
         $vsVersions = Get-ModuleContext "VsVersions"
         foreach($key in $Alias) {
            $vsVersions.Add($key, $Version)
         }  
    }