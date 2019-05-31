
Import-Module "$PsScriptRoot/../Gz-PasswordGenerator.psd1"

Describe "Gainz-PasswordGenerator" {

    It "Should generate new password" {
        $pw = New-GzPassword -AsString 
        $pw | Should Not Be $null 
        $pw -is [string] | Should Be $true
        
        $pw2 = New-GzPassword -AsString 
        $pw | Should Not Be $pw2 
    }

    It "Should generate a unique password" {
        Add-GzPasswordGeneratorAlias
        $r = new-object Random 
        $length = $r.Next(10, 10000)
        $list = @()
        for($i = 0; $i -lt $length; $i++) {
            $pw = New-Password -AsString
            $list.Contains($pw) | Should Be $false  
            $list += $pw;
        }
        Remove-GzPasswordGeneratorAlias
    }

    It "Should return a secure string" {
        $ss = New-GzPassword -AsSecureString 
        $ss -is [SecureString] | Should Be $true 
    }
}