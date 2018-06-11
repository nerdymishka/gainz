$passwordCharSets = @{
    LatinAlphaUpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    LatinAlphaLowerCase = "abcdefghijklmnopqrstuvwxyz";
    Digits = "0123456789";
    Hyphen = "-";
    Underscore = "_";
    Brackets = "[]{}()<>";
    Special = "~`&%$#@*+=|\/,:;^";
    Space = " ";
}

 function Merge-PasswordCharSets() {
    [cmdletbinding()]
    Param(
        [ValidateSet('LatinAlphaUpperCase', 'LatinAlphaLowerCase', 'Digits', 'Hyphen', 'Underscore', 'Brackets', 'Special', 'Space')]
        [Parameter()]
        [string[]] $CharSets
    )
    
    if($CharSets -eq $null -or $CharSets.Length -eq 0) { return $null }

    $result = $null;

    if($CharSets -ne $null -and $CharSets -gt 0) {
        $sb1 = New-Object System.Text.StringBuilder

        foreach($setName in $CharSets) {
            if($passwordCharSets.ContainsKey($setName)) {
             
                $characters = $passwordCharSets[$setName];
              
                $sb1.Append($characters) | Out-Null
            }
        }

        $result = $sb1.ToString();
    }
    return $result;
}


function Test-Password() {
    Param(
        [Char[]] $Characters,
        [ScriptBlock] $Validate
    )
    
    if($characters -eq $null -or $characters.Length -eq 0) {
        return $false;
    }

    if($Validate -ne $null) {
        & $Validate -Characters $Characters;
    }

    $lower = $false;
    $upper = $false;
    $digit = $false;

    for($i = 0; $i -lt $characters.Length; $i++) {
        if($lower -and $upper -and $digit) {
            return $true;
        }
        
        $char = [char]$characters[$i];


        if([Char]::IsDigit($char)) {
            $digit = $true;
            continue;
        }

        if([Char]::IsLetter($char)) {
            if([Char]::IsUpper($char)) {
                $upper = $true;
                continue;
            }

            if([Char]::IsLower($char)) {
                $lower = $true;
            }
        }
    }

    return $false;
}

function New-KwehPassword() {
    Param(
        [int] $Length,
        [ValidateSet('LatinAlphaUpperCase', 'LatinAlphaLowerCase', 'Digits', 'Hyphen', 'Underscore', 'Brackets', 'Special', 'Space')]
        [string[]] $CharSets = $null,
        [string] $Chars = $null,
        [ScriptBlock] $Validate = $null,
        [switch] $AsSecureString
    )

    if($Length -eq 0) {
        $Length = 16;
    }

    $sb = New-Object System.Text.StringBuilder
    if($CharSets -ne $Null -and $CharSets.Length -gt 0) {
        $set = (Merge-PasswordCharSets $CharSets)
        if($set -and $set.Length -gt 0) {
            $sb.Append($set) | Out-Null  
        }
    }

    if(![string]::IsNullOrWhiteSpace($Chars)) {
        $sb.Append($Chars)  | Out-Null  
    } 

    if($sb.Length -eq 0) {
        $sets = Merge-PasswordCharSets (@('LatinAlphaUpperCase', 'LatinAlphaLowerCase', 'Digits', 'Hyphen', 'Underscore'))
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
            $index = [int] ($bytes[$i] % $permittedChars.Length);
            $password[$i] = [char] $permittedChars[$index];
        }
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
Set-Alias -Name New-Password -Value New-KwehPassword 