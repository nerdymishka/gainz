
Import-module "$PsScriptroot/../Gz-VisualStudio.psd1" -Force


Describe "Gz-VisualStudio" {
    It "should read a visual studio solution" {
        $info = Read-VisualStudioSolution "$PsScriptRoot\Projects\Sample\Sample.sln" -All 
        
        $info.Version | Should Not Be $null
        $info.Version | Should Be "15.0.27703.1"
        $info.Projects | Should Not Be $null 

        $info.Projects["Sample.Lib"] | Should not be $null
        $proj = $info.Projects["Sample.Lib"]
        $proj.Name | Should be "Sample.Lib"
        $proj.Ext | Should be ".csproj"
        $proj.LanguageExt | Should Be ".cs"
        $proj.IsSdk | Should be $True 

        $proj2 = $info.Projects["WindowsService1"];
        $proj2 | Should Not Be $null 
        $proj2.IsWindowsService | Should Be $True 
        $proj2.IsSdk | Should be $false 
    }

    It "Should get a Visual Studio Path" {
        if($Env:Os -eq "Windows_NT")
        {
            if((Test-Path "${Env:ProgramFiles(x86)}\Microsoft Visual Studio")) {
                $path = Get-VisualStudioPath -Latest
                $path | Should Not Be $Null 
                $path.Contains("Microsoft Visual Studio\") | Should Be $True 
            } else {
                $path = Get-VisualStudioPath -Latest
                $path | Should Not Be $Null 
                $path.Contains("Microsoft Visual Studio") | Should Be $True 
            }
        }
    }

    It "Should get a ms build path" {
        $path = Get-MsBuildPath -Latest
        $path | Should Not Be $Null
        if($env:Os -eq "Windows_NT") {
            $path.Contains("MsBuild.exe") | Should Be $True 
        }    
    }


    It "Should build a solution" {
        $results = Invoke-VisualStudioBuild "$PsScriptRoot\Projects\Sample\Sample.sln" -NugetRestore -Redirect
        $results | Should Not Be $Null 
        if($results -is [Array]) {
            $results.Length | Should Be 1
            $results[0].ExitCode | Should Be 0
        } else {
            $results.ExitCode | Should Be 0
        }
    }

    It "Should run a test project" {
        $info = Read-VisualStudioSolution "$PsScriptRoot\Projects\Sample\Sample.sln" -All 
        $testProj = $info.Projects["XUnitTestProject1"]
        $proj = $testProj.File | Split-Path 

        $results = Invoke-VisualStudioTestConsole $proj -TestAssemblyPattern "**\*Test*.dll" -Redirect 
        if($results -is [Array]) {
            $results[0].ExitCode | Should Be 0
        } else {
            $results.ExitCode | Should Be 0 
        }
    }
}