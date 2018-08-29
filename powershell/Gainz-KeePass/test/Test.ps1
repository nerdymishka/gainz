 . "$PSScriptRoot\Load.ps1"

Describe "Module: $FmgModuleName" {
    $externalPW = "megatron"
    $pw = ConvertTo-SecureString $externalPW -AsPlainText -Force
    $defaultKdbx =  "$PSScriptRoot\NewDatabase.kdbx"
    $newKdbx = "$PsScriptRoot\Next.kdbx"
    $tmp = "$PsScriptRoot\tmp.txt"
    $tmp2 = "$PsScriptRoot\tmp2.txt"

    if(!(Test-Path $defaultKdbx)) {
        Write-Warning "could not locate $defaultKdbx"
        exit;
    }
    
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
            $db.MetaInfo.DatabaseName | Should Be "NewDatabase"
            $db.Dispose()
        }

        It "Should open with a scriptblock" {
            Open-KeePassPackage $defaultKdbx -Password ($pw) -Do {
                $_ | Should Not Be Null
                $_.MetaInfo.DatabaseName | Should Be "NewDatabase"
                $db | Should Not Be Null
                $db.MetaInfo.DatabaseName | Should Be "NewDatabase"
            }
        }
    }

    Context "New-KeePassPackage" {
        if(Test-Path $newKdbx) {
            Remove-Item $newKdbx -Force
        }
   
        It "Should create new KeePass Packge" {
            $key = New-KeePassKey -Password ($pw)
            $db = New-KeePassPackage -Key $key -Path $newKdbx
            $db | Should Not Be $Null 
            $db.GetType().FullName | Should Be "NerdyMishka.KeePass.KeePassPackage" 
        }
        
        if(Test-Path $newKdbx) {
            Remove-Item $newKdbx -Force
        }
    }

    Context "New-KeePassEntry" {
        if(Test-Path $newKdbx) {
            Remove-Item $newKdbx -Force
        }
   
        It "Should create new KeePass entry" {
            $key = New-KeePassKey -Password ($pw)
            $db = New-KeePassPackage -Key $key -Path $newKdbx

            # creates an entry and forces the creation of the groups
            $entry = $db | New-KeePassEntry "Next/Sub1/Sub2/gmail" -UserName "admin" `
                -Uri "www.google.com" -Notes "this is a test" -Force 

            $entry | Should Not Be $Null
            $entry.Name | Should Be "gmail"
            $entry.UserName | Should Be "admin"
            $entry.Url | Should Be "www.google.com"
            $entry.Notes | Should Be "this is a test"
            
            $db.Document.RootGroup.Name | Should Be "Next"
            $group = $db.FindGroup("Next/Sub1/Sub2")
            $group | Should Not Be $Null
        }
        
        if(Test-Path $newKdbx) {
            Remove-Item $newKdbx -Force
        }
    }

    Context "Import-KeePassBinary" {
        if(Test-Path $newKdbx) {
            Remove-Item $newKdbx -Force
        }

        if(Test-Path $tmp) {
            Remove-Item $tmp -Force
        }

        if(Test-Path $tmp2) {
            Remove-Item $tmp2 -Force
        }
   
        It "Should import and export a file" {
            $key = New-KeePassKey -Password ($pw)
            $db = New-KeePassPackage -Key $key -Path $newKdbx

            # creates an entry and forces the creation of the groups
            $entry = $db | New-KeePassEntry "Next/Sub1/Sub2/gmail" -UserName "admin" `
                -Uri "www.google.com" -Notes "this is a test" -Force 

            "my text file" | Out-File $tmp -Encoding "UTF8"
            $bytes = [System.IO.File]::ReadAllBytes($tmp)
            $entry | Import-KeePassBinary -Name "tmp.txt" -Data $bytes 

            $entry | Export-KeePassBinary -Name "tmp.txt" -DestinationPath $tmp2
            
            $content = Get-Content  $tmp2
            $content | Should Be "my text file"
        }
        
        if(Test-Path $newKdbx) {
            Remove-Item $newKdbx -Force
        }

        if(Test-Path $tmp) {
            Remove-Item $tmp -Force
        }

        if(Test-Path $tmp2) {
            Remove-Item $tmp2 -Force
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
                $entry.Name | Should Be "Sample Entry #2"
                $entry.UnprotectPassword() | Should Be "12345"
                #$entry.Password | Should Be "************"
                $entry.UserName | Should Be "Michael321"
            }
        }
    }

    Context "Find-KeePassEntry" {
       
        It "Find an entry by path" {
            Open-KeePassPackage $defaultKdbx -Password ($pw) -Do {
                $entry = $Package | Find-KeePassEntry "NewDatabase/Sample Entry #2"
                if($entry -eq $Null) {
                    Write-Warning "entry is null"
                }
                $entry | Should Not Be $Null 
                $entry.Name | Should Be "Sample Entry #2"
                $entry.UnprotectPassword() | Should Be "12345"
                #$entry.Password | Should Be "************"
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
