Import-Module "$PSScriptRoot/../Gainz-Core.psm1" -Force

Describe "Gainz-Core"  {

    Context "Test-IsElevated" {
        It "Should test if the current user is elevated" {
            switch([Environment]::OSVersion.Platform)  {
                "Win32NT" {
                    $isAdmin = Test-IsElevated;
                    $identity = [System.Security.Principal.WindowsIdentity]::GetCurrent()
                        $admin = [System.Security.Principal.WindowsBuiltInRole]::Administrator
                        $badTest = ([System.Security.Principal.WindowsPrincipal]$identity).IsInRole($admin)
                    if($isAdmin) {
                       
                        $badTest | Should Be $True 
                    } else {
                        $badTest | Should Be $false
                    }
                }
                "Unix" {
                    $content = id -u
                    $isAdmin = Test-IsElevated;
                    if($isAdmin) {
                        $content = id -u
                        $content |  should be "0"
                    } else {
                        $content | Should Not Be "0"
                    }
                }
            }
        }
    }


    Context "Test-Interactive" {
        IT "Should determine if the current session is interactive" {
            $args = [Environment]::GetCommandLineArgs()
            $active = $true;
            foreach($arg in $args) {
                if($arg -match "NonInteractive") {
                    $Env:GAINZ_INTERACTIVE = "0";
                    $active = $false;
                    break;
                }
            }
    
            $isActive = Test-Interactive;
            if($isActive) {
                $active | Should Be $True;
            } else {
                $active | Should Be $False;
            }

            
            $cmd = "& { 
                Import-Module `"$PsScriptRoot/../Gainz-Core.psm1`" -Force
                Test-Interactive 
            }"
            if([System.Environment]::OSVersion.Platform -eq "Win32NT") {
                $result = Powershell.exe -C $cmd -NonInteractive;
            } else {
                $result = pwsh -C $cmd -NonInteractive;
            }
         
            $result | Should Be "False";
        }
       
    }

}