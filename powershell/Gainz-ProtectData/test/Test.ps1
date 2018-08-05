Get-Item "$PsScriptRoot\..\private\*.ps1" | ForEach-Object {
    . "$($_.FullName)"
}

 
Get-Item "$PsScriptRoot\..\public\*.ps1" | ForEach-Object {
    . "$($_.FullName)"
}

Describe "Fmg-ProtectData" {

    $decryptedValue = "What's up, buttercup?"
   
    $key = [System.Text.Encoding]::UTF8.GetBytes("beefcake")

    It "Should encrypt a string" {
        $encryptedValue =  Protect-String -PrivateKey $key -Value $decryptedValue
        $encryptedValue | Should Not Be $decryptedValue
    }

    It "Should decrypt a string" {
        $encryptedValue =  Protect-String -PrivateKey $key -Value $decryptedValue
        $decryptedValue2 = Unprotect-String -PrivateKey $key -EncryptedValue $encryptedValue
        $decryptedValue2 | Should Be $decryptedValue
    }
}