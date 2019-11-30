

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

        $vsPaths = Get-VisualStudioPath -AsHashtable 
        foreach($key in $vsPaths.Keys) {
            if($key.StartsWith("buildtools:")) {
                $buildToolPaths.Add($key, $vsPaths[$key]);
            }
        }

        Set-ModuleVariable "BuildToolPaths" $buildToolPaths
    }

    
  

    if(![string]::IsNullOrWhiteSpace($Version)) {
        if($buildToolPaths.Count -eq 0) {
            return $null;
        }
        if($version.ToLower() -eq "latest") {
            $ceiling = $buildToolPaths.Keys | Sort-Object -Descending | Select-Object -First 1 
            return $buildToolPaths[$ceiling];
        }
        $versions = Get-ModuleVariable -Name "VsVersions"
        $v = $versions[$Version]

        return $buildToolPaths["buildtools:$v"]
    }

   
    if($AsHashtable.ToBool()) {
        return @{}
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

 