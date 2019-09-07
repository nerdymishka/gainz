function New-Password() {
    <#
    .SYNOPSIS
        Generates a new cryptographically secure password as a char array.

    .DESCRIPTION
        Generates a new cryptographically secure password with a given length
        using the supplied characters. 
    
    .PARAMETER Chars
        An string of characters to use to build the password.
        
    .PARAMETER CharSets
        Optional. An array of choices that leverages a group of characters to use to build
        the password. The default is 'LatinAlphaUpperCase', 'LatinAlphaLowerCase', 'Digits', 'SpecialHybrid'

    .PARAMETER Length
        Optional. The length of the password that is generated. The default is 16

    .PARAMETER Validate
        Optional. A script block that validates the password to ensure it meets
        standards. The default checks for one uppercase, one lowercase, one digit,
        one special character.
    
    .PARAMETER AsSecureString
        A flag that instructs the result to be returned as a SecureString.

    .PARAMETER AsString
        A flag that instructs the result to be returned as a string.
    
    .EXAMPLE
        PS C:\> $pw = New-Password -Length 16 -AsSecureString -CharSets 'LatinAlphaUpperCase', 'LatinAlphaLowerCase', 'Digits', 'SpecialHybrid'
        Generates a new password with 16 characters with 1 upper, 1 lower, 1 digit, and one special character.

    #>
    Param(

        [int] $Length,

        [ValidateSet('LatinAlphaUpperCase', 'LatinAlphaLowerCase', 'Digits', 'Hyphen', 'Underscore', 'Brackets', 'Special', 'Space')]
        [string[]] $CharSets = $null,
        
        [string] $Chars = $null,
        
        [ScriptBlock] $Validate = $null,
        
        [Switch] $AsSecureString,

        [Switch] $AsString
    )

    if($Length -eq 0) {
        $Length = 16;
    }

    $sb = New-Object System.Text.StringBuilder
    if(!$CharSets -and $CharSets.Length -gt 0) {
        $set = (Merge-PasswordCharSets $CharSets)
        if($set -and $set.Length -gt 0) {
            $sb.Append($set) | Out-Null  
        }
    }

    if(![string]::IsNullOrWhiteSpace($Chars)) {
        $sb.Append($Chars)  | Out-Null  
    } 

    if($sb.Length -eq 0) {
        $sets = Merge-PasswordCharSets -CharSets (@('LatinAlphaUpperCase', 'LatinAlphaLowerCase', 'Digits', "SpecialHybrid"))
        $sb.Append($sets)  | Out-Null  
    }

    $permittedChars = $sb.ToString();
    $password = [char[]]@(0) * $Length;
    $bytes = [byte[]]@(0) * $Length;


    while( (Test-Password $password -Validate $Validate) -eq $false) {
        $rng = [System.Security.Cryptography.RandomNumberGenerator]::Create()
        $rng.GetBytes($bytes);
        $rng.Dispose();

        for($i = 0; $i -lt $Length; $i++) {
            $byte = $bytes[$i]
            if($byte -eq 0) {
                $index = 0;
            } else {
                $index = [int]($byte % $permittedChars.Length)
            }
            $index = [int] ($bytes[$i] % $permittedChars.Length);
            $password[$i] = [char] $permittedChars[$index];
        }
    }

    
    if($AsString.ToBool()) {
        return  (-join $password)
    }
    
    if($AsSecureString.ToBool()) {
        $secureString = New-Object System.Security.SecureString;
        foreach($char in $password) {
            $secureString.AppendChar($char)
        }
        return $secureString;
    }

    # return char array as it is not immutable like a string
    return $password;
}