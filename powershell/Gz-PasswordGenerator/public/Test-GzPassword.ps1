function Test-GzPassword() {
    Param(
        [Char[]] $Characters,
        [ScriptBlock] $Validate
    )
    
    if(!$characters -or $characters.Length -eq 0) {
        return $false;
    }

    if($Validate -ne $null) {
        & $Validate -Characters $Characters;
    }

    $lower = $false;
    $upper = $false;
    $digit = $false;
    $special = $false;

    $others = "~`&%$#@*+=|\/,:;^_-[]{}()<> "

    for($i = 0; $i -lt $characters.Length; $i++) {
        if($lower -and $upper -and $digit -and $special) {
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

        if($others.Contains($char)) {
            $special = $true;
        }
    }

    return $false;
}