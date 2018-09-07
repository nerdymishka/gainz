function Get-VisualStudioTestConsolePath() {
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
        [Switch] $VisualStudio 
    )

    $vsTestPaths = Get-ModuleVariable -Name "VsTestPaths"

    if($vsTestPaths.ContainsKey($Version)) {
        return $vsTestPaths[$Version];
    }

    if([string]::IsNullOrWhiteSpace($Version)) {
        $Version = "Latest";
    }
    
    $paths = $null;

    if($VisualStudio.ToBool()) {
        $paths = Get-VisualStudioPath;
    } else {
        $paths = Get-BuildToolsPath;
        if(!$paths -or $Paths.Count -eq 0) {
            $paths = Get-VisualStudioPath;
        }
     }

     if($null -eq $paths -or $paths.Count -eq 0) {
        Write-Error "Unable to find an installed version of vstest console";
        return $null;
     }

     if($Version -eq "Latest") {
        $name = $paths.name;
        if($name -is [Array]) {
            $name = $name | Sort-Object -Descending | Select-Object -First 1 
        }
        $vsPath = $paths[$name]
     } else {
         $vsPath = $paths[$Version];
     }

      
    
    if([string]::IsNullOrWhiteSpace($vsPath)) {
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
Set-Alias -Name "Get-VsTestPath" -Value "Get-VisualStudioTestConsolePath"