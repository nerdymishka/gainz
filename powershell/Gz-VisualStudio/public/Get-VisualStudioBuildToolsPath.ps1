

$buildToolPaths = @{};
function Get-VisualStudioBuildToolsPath()  {
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [String] $Version,

        [Alias("l")]
        [Switch] $Latest,

        [Alias("f")]
        [Switch] $Force,

        [Alias("ht")]
        [Switch] $AsHashtable,

        [String] $VisualStudioBuildFolder = $null
    )

    if($Latest.ToBool())
    {
        $Version = "Latest"
    }

    $buildToolPaths = Get-ModuleVariable "BuildToolPaths"
    if(!$buildToolPaths)
    {
        $buildToolPaths = @{}

       

        if([string]::IsNullOrWhiteSpace($VisualStudioBuildFolder))
        {
            $VisualStudioBuildFolder = "$programFiles\Microsoft Visual Studio";
            $folders = Get-ChildItem "${Env:ProgramFiles(x86)}\Microsoft Visual Studio"
            foreach($folder in $folders) {
                [int] $year = 0
                if([int]::TryParse($folder.Name, [ref] $year)) {
                    $version = Get-VisualStudioVersion "$year" 
                    if($version)
                    {
                        if(Test-Path "$($folder.FullName)\BuildTools") {
                            $buildToolPaths.Add($version, "$($folder.FullName)\BuildTools")
                        }
                    }
                }
            }
        }

        Set-ModuleVariable "BuildToolPaths" $buildToolPaths
    }
  

    if(![string]::IsNullOrWhiteSpace($Version)) {
        if($version.ToLower() -eq "latest") {
            $ceiling = $buildToolPaths.Keys | Sort-Object -Descending | Select-Object -First 1 
            return $buildToolPaths[$ceiling];
        }

        return $buildToolPaths[$version]
    }

   
    if($AsHashtable.ToBool()) {
        return $buildToolPaths;
    }


    $result = @()
    foreach($name in $buildToolPaths.Keys) {
        $result += New-Object PsObject -Property @{
            Name = $name
            Path = $buildToolPaths[$name]
        }
    }

    # Handle Powershell's weird array conversion
    return ,$result
}

 