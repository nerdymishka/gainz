function Get-GzVisualStudioTestConsolePath() {
    <#
    .SYNOPSIS
        Gets the path for the visual studio test console for the
        specified version of Visual Studio.  
    .DESCRIPTION
        Gets the path for the visual studio test console for the
        specified version of Visual Studio. If no version is specified
        the latest version of Visual Studio found on the system is used.

    .PARAMETER Version
        The version of Visual Studio the cmdlet should lookup and 
        return the Visual Studio Test Console path. 

    .EXAMPLE
        PS C:\> $vsTestPath = Get-VisualStudioTestConsolePath

    .EXAMPLE
        PS C:\> $vsTestPath = Get-VsTestPath
        
    .OUTPUTS
        Returns the path for the Visual STudio Test Console
    #>
    Param(
        [Parameter(Position = 0)]
        [String] $Version, 
        [Switch] $VisualStudio,
        [Switch] $Latest
    )

    if($Latest.ToBool()) {
        $Version = "Latest"
    }

    $vsTestPaths = Get-ModuleVariable -Name "VsTestPaths"

    if($vsTestPaths.ContainsKey($Version)) {
        return $vsTestPaths[$Version];
    }

    if([string]::IsNullOrWhiteSpace($Version)) {
        $Version = "Latest";
    }
    
    $paths = $null;

    if($VisualStudio.ToBool()) {
        $paths = Get-GzVisualStudioPath -Latest;
    } else {
        $paths = Get-GzVisualStudioBuildToolsPath -Latest;
        if(!$paths -or $Paths.Count -eq 0) {
            $paths = Get-GzVisualStudioPath -Latest;
        }
     }

     if($null -eq $paths -or $paths.Count -eq 0) {
        Write-Error "Unable to find an installed version of vstest console";
        return $null;
     }

     if($Version -eq "Latest") {
       
        if($paths -is [Array]) {
            $vsPath = Sort-Object -Property Name -Descending | Select-Object -Property Path -First 1 
        } else {
            $vsPath = $paths;
        }
        
     } else {
         foreach($set in $paths)
         {
             if($set.Name -eq $Version) {
                $vsPath = $set.Path;
                break;
             }
         }
    }
    
    if([string]::IsNullOrWhiteSpace($vsPath)) {
        Write-Warning "vsPath not found";
        return $null;
    }
    
    $vsTest ="$vsPath\Common7\IDE\Extensions\TestPlatform\VSTest.Console.exe"
    
    if(Test-Path "$vsTest") {
        $vsTestPaths.Add($Version, $vsTest);
        return $vsTest 
    }

   

    $vsTest = "$vsPath\Common7\IDE\CommonExtensions\Microsoft\TestWindow\VSTest.Console.exe"

    if(Test-Path $vsTest) {
        $vsTestPaths.Add($Version, $vsTest);
        return $vsTest
    }
    
    $vsTestPaths.Add($Version, $null);

    return $null;
}