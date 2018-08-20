Get-Item "$PsScriptRoot\..\public\*.ps1" | ForEach-Object {
    . "$($_.FullName)"
}

Describe "Gainz-PasswordGenerator" {

    It "Should generate new password" {
        $pw = NEw-Password -AsString 
        $pw | Should Not Be $null 
        $pw -is [string] | Should Be $true
        
        $pw2 = New-Password -AsString 
        $pw | Should Not Be $pw2 
    }

    It "Should generate a unique password" {

        $r = new-object Random 
        $length = $r.Next(10, 10000)
        $list = @()
        for($i = 0; $i -lt $length; $i++) {
            $pw = New-Password -AsString
            $list.Contains($pw) | Should Be $false  
            $list += $pw;
        }
    }

    It "Should return a secure string" {
        $ss = New-Password -AsSecureString 
        $ss -is [SecureString] | Should Be $true 
    }
}