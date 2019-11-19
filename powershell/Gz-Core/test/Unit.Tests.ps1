

Import-Module "$PsScriptRoot/../*.psm1" -Force

InModuleScope "Gz-Core" {

    Describe "Test-64Bit" {
        
        It "Should be false when IntPtrSize is not 8" {
            Mock Get-IntPtrSize { return $false }
            Test-64Bit | Should Be $false 
        }

        It "Should be true when IntPtrSize is 8" {
            Mock Get-IntPtrSize { return $true } 
            Test-64Bit | Should Be $true 
        }
    }

    Describe "Test-OsPlatform" {

        It "Should return true with default parameters and platform is Win32Nt" {
            Mock Get-OsPlatform {return "Win32NT" }
            Test-OsPlatform | Should Be $True 

            Mock Get-OsPlatform { return "Win32s" }
            Test-OsPlatform | Should Be $True

            Mock Get-OsPlatform {return "Win32Windows" }
            Test-OsPlatform | Should Be $True 

            Mock Get-OsPlatform {return "Unix" }
            Test-OsPlatform | Should Be $False
            
            Mock Get-OsPlatform {return "MacOSX" }
            Test-OsPlatform | Should Be $False 
        }

        It "Should return true for Windows platform(s)" {
            Mock Get-OsPlatform {return "Win32NT" }
            Test-OsPlatform "Windows" | Should Be $True 

            Mock Get-OsPlatform { return "Win32s" }
            Test-OsPlatform "Windows" | Should Be $True 

            Mock Get-OsPlatform {return "Win32Windows" }
            Test-OsPlatform "Windows" | Should Be $True 

            Mock Get-OsPlatform {return "Unix" }
            Test-OsPlatform "Windows" | Should Be $False
            
            Mock Get-OsPlatform {return "MacOSX" }
            Test-OsPlatform "Windows" | Should Be $False 
        }

        It "Should return true for Linux or Unix platform(s)" {
            Mock Get-OsPlatform {return "Win32NT" }
            Test-OsPlatform "Linux" | Should Be $False  

            Mock Get-OsPlatform { return "Win32s" }
            Test-OsPlatform "Linux" | Should Be $FAlse 

            Mock Get-OsPlatform {return "Win32Windows" }
            Test-OsPlatform "Linux" | Should Be $False 

            Mock Get-OsPlatform { return "Unix" }
            Test-OsPlatform "Linux" | Should Be $True 
            
            Mock Get-OsPlatform {return "MacOSX" }
            Test-OsPlatform "Linux" | Should Be $False 

        }

        It "Should return true for Mac platform(s)" {
            Mock Get-OsPlatform {return "Win32NT" }
            Test-OsPlatform "Mac" | Should Be $False  

            Mock Get-OsPlatform { return "Win32s" }
            Test-OsPlatform "Mac" | Should Be $FAlse 

            Mock Get-OsPlatform {return "Win32Windows" }
            Test-OsPlatform "Mac" | Should Be $False 

            Mock Get-OsPlatform { return "Unix" }
            Test-OsPlatform "Mac" | Should Be $False 
            
            Mock Get-OsPlatform {return "MacOSX" }
            Test-OsPlatform "Mac" | Should Be $True  

        }
    }


    Describe "Test-UserIsAdministrator" {

        It "Should return true with windows user is an administrator" {
            Mock Get-OsPlatform { return "Win32NT" }
            Mock Test-WinUserIsInRole `
                 -ParameterFilter { $BuiltInRole -eq "Administrator" } `
                 -MockWith { return $true }

            Test-WinUserIsInRole -BuiltInRole "Administrator" | Should Be $True 

            Test-UserIsAdministrator -Force | Should Be $true 
        }

        It "Should return false when windows user is not an administrator " {
            Mock Get-OsPlatform { return "Win32NT" }
            Mock Test-WinUserIsInRole `
                 -ParameterFilter { $BuiltInRole -eq "Administrator" } `
                 -MockWith { return $false }

            Test-WinUserIsInRole -BuiltInRole "Administrator" | Should Be $False 

            Test-UserIsAdministrator -Force | Should Be $false 
        }

        IT "Should return true when linux user is root" {
            Mock Get-OsPlatform { return "Unix" }
            Mock Test-UnixUserIsInRole `
                 -ParameterFilter { $Group -eq "Root" } `
                 -MockWith { return $true }

            Test-UnixUserIsInRole -Group "Root" | Should Be $True 

            Test-UserIsAdministrator -Force | Should Be $true 
        }

        IT "Should return true when linux user is root" {
            Mock Get-OsPlatform { return "Unix" }
            Mock Test-UnixUserIsInRole `
                 -ParameterFilter { $Group -eq "Root" } `
                 -MockWith { return $false }

            Test-UnixUserIsInRole -Group "Root" | Should Be $false 

            Test-UserIsAdministrator -Force | Should Be $false 
        }
    }

    Describe "New-DynamicParameter" {
        It "Should create a new dynamic runtime parameter" {
            $parameter = New-DynamicParameter "Name"
            $parameter | Should Not Be $Null 
            $parameter.Name | Should Be "Name"

            $t = [int32]
        

            $parameter = New-DynamicParameter "Name" $t  3  -FromPipeline -Aliases "n"

            $parameter.Name | Should Be Name 
            $Parameter.ParameterType | Should Be $t
            $parameter.Attributes | Should Not Be $Null 
            $parameter.Attributes.Count | Should Be 2 

            $attr = $parameter.Attributes[0];
            $attr.ValueFromPipeline | Should Be $True;
            $attr.ValueFromPipelineByPropertyName | Should Be $False;
            $attr.Mandatory | Should Be $False;
            
            $attr2 = $parameter.Attributes[1];
            $attr2.AliasNames[0] | Should Be "n";
        }
    }

    Describe "Add-DynamicParameter" {
        It "Should add a dynamic parameter to an existing runtime parameter dictionary" {
            $Dictionary = New-Object System.Management.Automation.RuntimeDefinedParameterDictionary
            $Dictionary | Add-DynamicParameter (New-DynamicParameter "Bob")
            $Dictionary | Should Not Be $Null 

            $Dictionary.ContainsKey("Bob") | Should Be $True 
            $Dictionary["Bob"] | Should Not Be $Null 
        }

        It "Should add a dynamic parameter to a new runtime parameter dictionary using -PassThru" {
            $Dictionary = Add-DynamicParameter (New-DynamicParameter "Bob") -PassThru
            $Dictionary | Should Not Be $Null 

            $Dictionary.ContainsKey("Bob") | Should Be $True 
            $Dictionary["Bob"] | Should Not Be $Null 
        }
    }

    Describe "Write-GzModuleSetting" {
        It "Should write a value to a configuration file" {
            Write-GzModuleSetting "Write/Test" 1 -Storage "$PsScriptRoot/Resources"
            Write-GzModuleSetting "Write/Test2" "Hola" -Storage "$PsScriptRoot/Resources"


            $file = "$PsScriptRoot/Resources/gz.json"
            Test-Path $file | Should Be $True
            
            $config = Get-Content $file -Raw | ConvertFrom-Json 
            $config.Write.Test | Should Be 1
            $config.Write.Test2 | Should Be "Hola"
        }
    }

    Describe "Read-GzModuleSetting" {
        It "Should read a value from the configuration variable in memory" {
            $one = Read-GzModuleSetting "Write/Test"  "bad"  -Storage "$PsScriptRoot/Resources"
            $two = Read-GzModuleSetting "Write/Test2" "bad"  -Storage "$PsScriptRoot/Resources"


            $one | Should Be 1
            $two | Should Be "Hola"
        }

        It "Should read a value from the configuration file" {
            $one = Read-GzModuleSetting "Write/Test"  "bad"  -Storage "$PsScriptRoot/Resources" -Force 
            $two = Read-GzModuleSetting "Write/Test2" "bad"  -Storage "$PsScriptRoot/Resources" -Force


            $one | Should Be 1
            $two | Should Be "Hola"
        }
    }


    if(Test-Path "$PsScriptRoot/Resources") {
       Remove-Item "$PsScriptRoot/Resources" -Force -Recurse | Write-Debug
    }
}