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
        [String] $Version 
    )

    $vsTestPaths = Get-ModuleVariable -Name "VsTestPaths"

    if($vsTestPaths.ContainsKey($Version)) {
        return $vsTestPaths[$Version];
    }

    if([string]::IsNullOrWhiteSpace($Version)) {
        $vsPath = Get-VisualStudioPath -Latest
    } else {
        $vsPath = Get-VisualStudioPath -Version $Version
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