

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

    <#
    if([string]::IsNullOrWhiteSpace($MsBuildFolder))
    {
        $programFiles = ${Env:ProgramFiles(x86)}
        if(!$programFiles) { $programFiles = $Env:ProgramFiles }

        $msBuildfolder = "$programFiles\MSBuild"
    }
  

    if((Test-Path $msBuildfolder))
    {
        $folders = Get-ChildItem "$programFiles\MSBuild"
        foreach($folder in $folders)
        {
            $version = 0
            if([decimal]::TryParse($folder.Name, [ref] $version)) {
                $buildToolPaths.Add($folder.Name, $folder.FullName)
            }
        }
    }#>

    if([string]::IsNullOrWhiteSpace($Version))
    {

    }


    if($buildTools) {
        $root = $null;
        $key = $null;
        if($Version -eq "latest") {
            $name = $buildTools.Name;
            if($name -is [Array]) {
                $name = $buildTools.Name | Sort-Object -Descending | Select-Object -First 1 
            }

            foreach($x in $buildTools) {
                if($name -eq $x.Name) {
                    $root = $x.Path;
                    break;
                }
            }
        }
        
        if(![string]::IsNullOrWhiteSpace($Version)) {
            $key = $Version;
            $root = $buildTools[$Version].Path;
        }


        if($null -ne $root) {
            

            return "$root\MsBuild\$key\Bin\MsBuild.exe"
        }
  
    }

    if($Latest.ToBool() -or $Version -eq "latest") {
        $paths = Get-GzVisualStudioPath
        if($paths) {
            $Version = $paths.Keys | Sort-Object -Descending | Select-Object -First 1 
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

            $names = Get-I

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

    if($Version -or $Latest.ToBool()) {
        
        if($Latest.ToBool() -or $Version -eq "latest") {
            if(!$paths) {
                $paths = Get-GzVisualStudioPath
                if(!$path) { return $null }
            }
            

            $v = $paths | Sort-Object -Property Name -Descending 
            $item = $v[0]
            $Version = $item.Name
        }

        $vsPath = Get-GzVisualStudioPath -Version $Version 
        if($vsPath -is [string]) {
            if(Test-Path "$vsPath\MsBuild\$Version\Bin\MsBuild.exe") {
                return "$vsPath\MsBuild\$Version\Bin\MsBuild.exe"
            }
        }
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