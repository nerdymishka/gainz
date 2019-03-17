
$buildToolsVersions = @{
    "15" = "15.0"
    "15.0" = "15.0"
    "2017" = "15.0"
    "Visual Studio 2017" = "15.0"
}
$buildToolPaths = @{};
function Get-GzVisualStudioBuildToolsPath()  {
    Param(
        [String] $Version,

        [Switch] $Latest 
    )

    $buildToolPaths = Get-ModuleVariable "BuildToolPaths"
    if(!$buildToolPaths)
    {
        $buildToolPaths = @{}
        $root = "${Env:ProgramFiles(x86)}\Microsoft Visual Studio";
        $folders = Get-ChildItem "${Env:ProgramFiles(x86)}\Microsoft Visual Studio"
        foreach($folder in $folders) {
            [int] $year = 0
            if([int]::TryParse($folder.Name, [ref] $year)) {
                
                if($year -eq 2017) {
                    $v = "15.0"
                    if(Test-Path "$root\$year\BuildTools") {
                        $buildToolPaths.Add($v, "$root\$year\BuildTools")
                    }
                }
            }
        } 

        Set-ModuleVariable "BuildToolPaths" $buildToolPaths
    }

    if($Latest.ToBool())
    {
        $Version = "Latest"
    }

    if(![string]::IsNullOrWhiteSpace($Version)) {
        if($version.ToLower() -eq "latest") {
            $ceiling = $buildToolPaths.Keys | Sort-Object -Descending | Select-Object -First 1 
            return $buildToolPaths[$ceiling];
        }

        return $buildToolPaths[$version]
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

 