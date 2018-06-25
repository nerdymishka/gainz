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
    $path = Get-VisualStudioPath "Visual Studio 2017"

    .EXAMPLE
    $path = Get-VisualStudioPath "2017"

    .EXAMPLE
    $path = Get-VisualStudioPath "15" # still Visual Studio 2017

    .EXAMPLE
    $paths = Get-VisualStudioPath # Gets all paths for VisualStudio versions that are installed 
#>
    [CmdletBinding()]
    Param(
        [Parameter(Position = 0)]
        [string] $Version,
    
        [Switch] $Latest 
    )

    if($Latest.ToBool()) {
        $Version = "latest"
    }

    $vsPaths = Get-ModuleVariable "VsPaths"

    
    if($vsPaths -eq $null) {
        $versionJson = $null;
        $platform = [System.Environment]::OSVersion.Platform

        if($platform -eq "Win32NT") {
            $vsPaths = @{}
            $key = Get-Item "HKLM:\SOFTWARE\WOW6432Node\Microsoft\VisualStudio\Sxs\VS7"

            foreach($entryName in $key.GetValueNames()) {
                [Decimal] $v = 0; 

                if([Decimal]::TryParse($entryName, [ref] $v)) {
                    $added = $false;

                    if($v -gt 14) {
                        if($versionJson -eq $null) {
                            $vsWhere = Get-Command "vswhere.exe" -ErrorAction SilentlyContinue
                            if($vsWhere) {
                                $vsWhere = $vsWhere.Path 
                            } else {
                                $vsWhere = "${Env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" 
                            }

                            if(Test-Path $vsWhere) {
                                $versionJson = & $vsWhere -format json 
                                $versionJson = $versionJson | ConvertFrom-Json 
                            } else {
                                $versionJson = @()
                            }
                        }
                    
                        $added = $false;
                        foreach($v2 in $versionJson) {
                            
                            if($v2.installationVersion) {
                                $major = $v.ToString()
                                $major = $major.Substring(0, $major.IndexOf("."))
           
                                if($v2.installationVersion.StartsWith($major)) {
                                        

                                    if($v2.isPrerelease) {
                                        $vsPaths.Add("$entryName-Pre", $v2.installationPath);
                                    } else {
                                        $vsPaths.Add("${entryName}", $v2.installationPath);
                                    }
                                    $added =$true;
                                }
                            }
                        }
                    }

                    if($added) {
                        continue;
                    }
               
                    $path = $key.GetValue($entryName);
                    $vsPaths.Add($entryName, $path);
                }
            }

            Set-ModuleVariable -Name "VsPaths" -Value $vsPaths
        } else {
            Write-Error "Platform $platform is not currently supported"
        }
        # TODO: mac version
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

Set-Alias -Name Get-VSPath -Value Get-VisualStudioPath 