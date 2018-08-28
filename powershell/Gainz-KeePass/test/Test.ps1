 . "$PSScriptRoot\Load.ps1"

Describe "Module: $FmgModuleName" {
    $externalPW = "harvey-spectre8$"
    $pw = ConvertTo-SecureString $externalPW -AsPlainText -Force
    $defaultKdbx =  "$PSScriptRoot\tests.kdbx"

    
    Context "New-KeePassKey" {

        It "Should create a new KeePass Master Key Object" {
            
            $key = New-KeePassKey -Password $pw
            $key | Should Not Be $Null 
            
            ($key.GetType().FullName) | Should Be "NerdyMishka.KeePass.MasterKey"
        }
    }

    Context "Open-KeePassPackage" {
       

        It "Should open a KeePass Package" {
            $key = New-KeePassKey -Password ($pw)
            $db = Open-KeePassPackage -Key $key -Path $defaultKdbx
            $db | Should Not Be $Null 
            $db.Meta.DatabaseName | Should Be "tests"
            $db.Dispose()
        }

        It "Should open with a scriptblock" {
            Open-KeePassPackage $defaultKdbx -Password ($pw) -Do {
                $_ | Should Not Be Null
                $_.Meta.DatabaseName | Should Be "tests"
                $db | Should Not Be Null
                $db.Meta.DatabaseName | Should Be "tests"
            }
        }
    }

    Context "Find-KeePassEntryByTitle" {
       

        It "Should open a KeePass Package" {
            Open-KeePassPackage $defaultKdbx -Password ($pw) -Do {
                $entry = $Package | Find-KeePassEntryByTitle "Sample Entry #2"
                if($entry -eq $Null) {
                    Write-Warning "entry is null"
                }
                $entry | Should Not Be $Null 
                $entry.Title | Should Be "Sample Entry #2"
                $entry.UnprotectPassword() | Should Be "12345"
                $entry.Password | Should Be "************"
                $entry.UserName | Should Be "Michael321"
            }
        }
    }

    <#
    Context "Export-KeePassBinary" {
        $cfg = Get-Content "$resources/fmg.json" -Raw | ConvertFrom-Json

        It "Should export a binary file by name" {
            Open-KeePassPackage $kdbxFile -Password ($pw) -Do {
                $entry = $_ | Find-KeePassEntryByTitle "Sample Entry #2"
                $entry | Should Not Be $Null 
                $entry | Export-KeePassBinary "test.txt" -DestinationPath "$PsScriptRoot/../Resources"
            
                $fileExist = (Test-Path "$PsScriptRoot/../Resources/test.txt")
                if($fileExist) {
                    Remove-Item "$PsScriptRoot/../Resources/test.txt" -Force
                }
            
                $fileExist | Should Be $true
            }
        }
    }#>
}
