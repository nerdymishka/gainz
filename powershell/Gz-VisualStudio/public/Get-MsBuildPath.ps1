

function Get-MsBuildPath() {
    [CmdletBinding()]
    Param(
        [string] $Version,
        [Switch] $VisualStudio,
        [Switch] $Latest 
    )

    if($Latest.ToBool()) {
        $Version = "Latest"
    }

 
    $paths = $null;
    $buildTools = $null;

    if(!$VisualStudio.ToBool()) {
        $buildTools = Get-VisualStudioBuildToolsPath 
    }

    if($Version -or $Latest.ToBool()) {
        if($buildTools) {
            $key = $version | Get-VisualStudioVersion
            $root = Get-VisualStudioBuildToolsPath -Version $key 
            $majorString = $key.Substring(0, $key.IndexOf("."));
            $major = $null 
            [void][int]::TryParse($majorString, [ref] $major)
            if($null -ne $major -and $major -gt 15) {
                return "$root\MsBuild\Current\Bin\MsBuild.exe"
            }
            if($null -ne $root) {
                return "$root\MsBuild\$key\Bin\MsBuild.exe"
            }
        }

        if($Latest.ToBool()) {
            $Version = "latest"
        }
        $key= $Version | Get-VisualStudioVersion 
        $majorString = $key.Substring(0, $key.IndexOf("."));
        $major = $null 
        [void][int]::TryParse($majorString, [ref] $major)
        $vsPath = Get-VisualStudioPath -Version $key
        if($vsPath -is [string]) {
            if($null -ne $major -and $major -gt 15) {
                return "$vsPath\MsBuild\Current\Bin\MsBuild.exe"
            }
            if(Test-Path "$vsPath\MsBuild\$Version\Bin\MsBuild.exe") {
                return "$vsPath\MsBuild\$Version\Bin\MsBuild.exe"
            }
        }
    }


    $msBuildPaths = Get-ModuleVariable -Name "MsBuildPaths"

    if(![string]::IsNullOrWhiteSpace($Version) -and $msBuildPaths.ContainsKey($Version)) {
        return $msBuildPaths[$Version];
    }

    if([string]::IsNullOrWhiteSpace($Version) -and !$Latest.ToBool()) {
        if($msBuildPaths.Count -gt 0) {
            return $msBuildPaths
        }

        if(!$paths) {
            $paths = Get-GzVisualStudioPath
            if(!$path) { return $null }     
        }
            
        $builds = @{};
       
        foreach($item in $paths) {
            $key = $v = $item.Name 
            $path = $item.Path 

            if(Test-Path "$path\MsBuild\$key\Bin\MsBuild.exe") {
                $builds.Add($key, "$path\MsBuild\$key\Bin\MsBuild.exe")
                continue;
            }

            $key = Get-Item "HKLM:\SOFTWARE\Microsoft\MSBuild\ToolsVersions\$key" -ErrorAction SilentlyContinue
            if($key) {
                $path = $key.GetValue("MSBuildToolsPath")
                if(!$path.EndsWith("\bin")) {
                    $path = Split-Path $path;
                }
                
                if(Test-Path "$path\MsBuild.exe") {
                    
                    $builds.Add($v, "$path\MsBuild.exe");
                }
            }
        }

        foreach($key in $builds.Keys) {
            if($msBuildPaths.ContainsKey($key)) {
                $msBuildPaths.Add($key, $builds[$key])
            }
        }

        return $builds;
    }

    
   
    $versions = @("14.0", "12.0", "10.0", "4.0", "3.0", "2.0")
    foreach($v in $versions) {
        $key = Get-Item "HKLM:\SOFTWARE\Microsoft\MSBuild\ToolsVersions\$v" -ErrorAction SilentlyContinue
        if($key) {
            $path = $key.GetValue("MSBuildToolsPath")
            
            if(!$path.EndsWith("\bin")) {
                $path = Split-Path $path;
            }
            
            if(Test-Path "$path\MsBuild.exe") {
                $msBuildPaths.Add($v, "$path\MsBuild.exe")
                return "$path\MsBuild.exe";
            }
        }
    }
    
    return $null;
}