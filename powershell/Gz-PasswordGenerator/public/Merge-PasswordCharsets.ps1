$passwordCharSets = @{
    LatinAlphaUpperCase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
    LatinAlphaLowerCase = "abcdefghijklmnopqrstuvwxyz";
    Digits = "0123456789";
    Hyphen = "-";
    Underscore = "_";
    Brackets = "[]{}()<>";
    Special = "~`&%$#@*+=|\/,:;^";
    SpecialHybrid = "_-#|^{}$";
    Space = " ";
}

 function Merge-PasswordCharSets() {
    [cmdletbinding()]
    Param(
        [ValidateSet('LatinAlphaUpperCase', 'LatinAlphaLowerCase', 'Digits', 'Hyphen', 'Underscore', 'Brackets', 'Special', 'Space', "SpecialHybrid")]
        [Parameter()]
        [string[]] $CharSets
    )
    
    if($null -eq $CharSets -or $CharSets.Length -eq 0) { return $null }

    $result = $null;
    $sb1 = New-Object System.Text.StringBuilder

    foreach($setName in $CharSets) {
        if($passwordCharSets.ContainsKey($setName)) {
            
            $characters = $passwordCharSets[$setName];
            
            $sb1.Append($characters) | Out-Null
        }
    }

    $result = $sb1.ToString();

    return $result;
}