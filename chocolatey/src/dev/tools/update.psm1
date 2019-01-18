$cfg = "$HOME/.config//install.json"
$config = $null;

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


function Test-Password() {
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
function Read-InstallConfig() {

    if($config) {
        return $config;
    }
   
    if(!(Test-Path $cfg)) {
        return @{}
    }

    $config = Get-Content $cfg -Raw | ConvertFrom-Json 
    $data = @{}
    $config.psobject.properties | Foreach { $data[$_.Name] = $_.Value }
    return $data;
}

function Write-InstallConfig() {
    Param(
        [Parameter(Position = 0)]
        [Hashtable] $data
    )

    if(!(Test-Path $cfg)) {
        Write-Host $cfg;
        $dir = Split-Path $cfg 
        mkdir $dir -Force
    }

    $c =  $data | ConvertTo-Json 
    $c | Out-File -FilePath $cfg -Encoding "UTF8" -Force
}

function Test-Checkpoint() {
    Param(
        [Parameter(Position = 0)]
        [String] $CheckPoint
    )

    $cfg = Read-InstallConfig

    if($cfg.ContainsKey($CheckPoint)) {
        return $true;
    }

    return $false;
}

function Save-CheckPoint() {
    Param(
        [Parameter(Position = 0)]
        [String] $CheckPoint,

        [Parameter(Position = 1)]
        [String] $ArgumentList
    )


    $data = Read-InstallConfig
    $data.Add($CheckPoint, $ArgumentList)
    Write-InstallConfig $data
}

function Invoke-ChocolateyInstall() {
    Param(
        [Parameter(Position = 0)]
        [String] $PackageName,

        [Parameter(Position = 1)]
        [String[]] $ArgumentList
    )

   

    if(Test-Checkpoint $PackageName) {
        
    }

    & choco install $PackageName @ArgumentList -y

    if($LASTEXITCODE -eq 0) {
       $str = [string]::Join(" ", $ArgumentList)
       Save-CheckPoint $PackageName -ArgumentList $str
    }
}

Export-ModuleMember -Function @(
    'Save-CheckPoint',
    'Test-Checkpoint',
    'Invoke-ChocolateyInstall',
    'New-Password'
)