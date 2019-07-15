function Get-VisualStudioPath() {
<# 
    .SYNOPSIS 
    Gets the Visual Studio path for a specific version or all the paths for 
    Visual Studio installs on the current machine.

    .DESCRIPTION
    Get-VisualStudioPath gets the base install path for various versions of 
    Visual Studio that is installed on the local machine.

    This is useful to build paths to executables such as devenvcom.  

    .PARAMETER Version
    The version number or alias of the Visual Studio version's install path
    you wish to retrieve if installed.

    .EXAMPLE
    $path = Get-GzVisualStudioPath "Visual Studio 2017"

    .EXAMPLE
    $path = Get-GzVisualStudioPath "2017"

    .EXAMPLE
    $path = Get-GzVisualStudioPath "15" # still Visual Studio 2017

    .EXAMPLE
    $paths = Get-GzVisualStudioPath # Gets all paths for VisualStudio versions that are installed 
#>
    [CmdletBinding()]
    Param(
        [Parameter(Position = 0, ValueFromPipelineByPropertyName = $true, ValueFromPipeline = $true)]
        [string] $Version,
    
        [Switch] $Latest,

        [Switch] $Force,

        [Switch] $AsHashtable
    )

    if($Latest.ToBool()) {
        $Version = "latest"
    }

    if(! (Test-OsPlatform "Windows")) {
        $plat = [System.Environment]::OSVersion.Platform
        Write-Debug "$plat does not support devenv.exe (Visual Studio)";
        
        if([String]::IsNullOrWhiteSpace($Version)) {
            return @{}
        }

        return $null;
    }

    $vsPaths = Get-ModuleVariable "VsPaths"
    
    if($null -eq $vsPaths -or $Force.ToBool()) {
        $versionJson = $null;
        $platform = [System.Environment]::OSVersion.Platform
        $vsPaths = @{}
            
        $vsPaths = Read-VsWherePathData
        if(!$vsPaths -or $vsPaths.Count -eq 0)
        {
         
            $vsPaths = @{}
            $installPaths = Get-ItemProperty "HKLM:\SOFTWARE\WOW6432Node\Microsoft\VisualStudio\Sxs\VS7" -EA SilentlyContinue
            if($installPaths) {
                $names = $installPaths.PSObject.Properties.Name 

                foreach($name in $names) {
                    [Decimal] $v = 0;
                    if([Decimal]::TryParse($name, [ref] $v)) {
                        $vsPaths.Add($name, $installPaths.$name )
                    }
                }
            }
        }

        Set-ModuleVariable -Name "VsPaths" -Value $vsPaths
        
    }

    $vsVersions = Get-ModuleVariable -Name "VsVersions"

    if(![string]::IsNullOrWhiteSpace($Version)) {
      
        if($vsPaths.Count -eq 0) {
            return $null;
        }

        if($version.ToLower() -eq "latest") {
           $ceiling = $vsPaths.Keys | Sort-Object -Descending | Select-Object -First 1 
           return $vsPaths[$ceiling];
        }

        if($vsVersions.ContainsKey($Version)) {
            $Version = $vsVersions[$Version];
        }
        if($vsPaths.Contains($Version)) {
            return $vsPaths[$version];
        }

        return $null;
    }

    if($AsHashtable.ToBool()) {
        return $vsPaths;
    }

    $result = @()
    foreach($name in $vsPaths.Keys) {
        $result += New-Object PsObject -Property @{
            Name = $name
            Path = $vsPaths[$name]
        }
    }

    # Handle Powershell's weird array conversion
    return ,$result
}