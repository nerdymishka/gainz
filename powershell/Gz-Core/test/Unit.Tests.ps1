

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

            Test-OsPlatform | Should Be $False 

            Mock Get-OsPlatform {return "Win32Windows" }

            Test-OsPlatform | Should Be $False 

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
}