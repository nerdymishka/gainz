


Import-Module "$PsScriptRoot/../*.psm1" -Force



InModuleScope "Gz-Core" {

    $isAdmin = Test-USerIsAdministrator
    if(!$isAdmin) {
        throw "Tests must be run by an admin"
    }

    function Write-TestFiles() {
        Param(
            [Parameter(Position = 0)]
            [String] $Name 
        )
    
        Remove-TestFiles -Name $Name 
    
        mkdir "$Name"
        mkdir "./$Name/sub1"
        mkdir "./$Name/sub2"
        mkdir "./$Name/sub1/nested1"
        mkdir "./$Name/sub2/nested1"
        "" > "./$Name/file1.txt"
        "" > "./$Name/file2.txt"
        "" > "./$Name/sub1/file1.txt"
        "" > "./$Name/sub1/file2.txt"
        "" > "./$Name/sub2/file1.txt"
        "" > "./$Name/sub2/file2.txt"
    }
    
    function Remove-TestFiles() {
        PAram(
            [Parameter(Position = 0)]
            $Name 
        )
    
        if(Test-Path $Name) {
            Remove-ITem $Name -Force -Recurse
        }
    }

    Describe "Set-XFileOwner" {
        Write-TestFiles "fo"

        if(Test-OsPlatform "Windows") {
            It "Should set the file owner & group of a single file" {
                $file = "./fo/file2.txt"
                Test-Path $file | Should Be $True 
                $acl = (Get-Acl $file)
                
                $acl.Owner | Should Be "BUILTIN\Administrators"
               
                
                Set-XFileOwner "BUILTIN\Users" $file 
    
                $acl = (Get-Acl $file)
                $acl.Owner | Should Be "BUILTIN\Users"
                $acl.Group | Should Be "BUILTIN\Users"
            }
    
            It "Should set the file owner & group of multipe files" {
                $file = "./fo/sub2"
                Test-Path $file | Should Be $True 
               
                $acl = (Get-Acl $file)
                $acl.Owner | Should Be "BUILTIN\Administrators"
               
    
                $acl = (Get-Acl "$file\file1.txt")
                $acl.Owner | Should Be "BUILTIN\Administrators"
              
    
                $acl = (Get-Acl "$file\file2.txt")
                $acl.Owner | Should Be "BUILTIN\Administrators"
               
                $acl = (Get-Acl "$file\nested1")
                $acl.Owner | Should Be "BUILTIN\Administrators"
                
                Set-XFileOwner "BUILTIN\Users" $file -Recurse
    
                $acl = (Get-Acl $file)
                $acl.Owner | Should Be "BUILTIN\Users"
                $acl.Group | Should Be "BUILTIN\Users"
    
                $acl = (Get-Acl "$file\file1.txt")
                $acl.Owner | Should Be "BUILTIN\Users"
                $acl.Group | Should Be "BUILTIN\Users"
    
                $acl = (Get-Acl "$file\file2.txt")
                $acl.Owner | Should Be "BUILTIN\Users"
                $acl.Group | Should Be "BUILTIN\Users"
    
                $acl = (Get-Acl "$file\nested1")
                $acl.Owner | Should Be "BUILTIN\Users"
                $acl.Group | Should Be "BUILTIN\Users"
    
            }
    
            It "Should set the file owner & group that are different " {
                $file = "./fo/file1.txt"
                Test-Path $file | Should Be $True 
               
                $acl = (Get-Acl $file)
                $acl.Owner | Should Be "BUILTIN\Administrators"
              
                
                Set-XFileOwner "BUILTIN\Users:BUILTIN\Guests" $file 
    
                $acl = (Get-Acl $file)
                $acl.Owner | Should Be "BUILTIN\Users"
                $acl.Group | Should Be "BUILTIN\Guests"
    
                Set-XFileOwner "BUILTIN\Users" -Group "BUILTIN\Administrators" "./fo/sub1/file1.txt"
    
                $acl = (Get-Acl "./fo/sub1/file1.txt")
                $acl.Owner | Should Be "BUILTIN\Users"
                $acl.Group | Should Be "BUILTIN\Administrators"
            }
        }

        Remove-TestFiles -Name "fo"
    }

}