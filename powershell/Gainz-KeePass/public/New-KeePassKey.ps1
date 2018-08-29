

function New-KeePassKey() {
<#
    .SYNOPSIS 
    Creates a new KeePass master key

    .DESCRIPTION
    The master key is a composite key that are required to open KeePass
    database files. Without it, the file can not be decrypted.

    .PARAMETER Password
    (Optional) The password for the composite key.

    .PARAMETER KeyFile
    (Optional) A file of bytes for the composites key.

    .EXAMPLE
    $key = New-KeePassKey -Passsword "your pass phrase"

    .EXAMPLE
    $key = New-KeePassKey -KeyFile "$home/Desktop/KP.key"

#>
    Param(
        [SecureString] $Password = $null,
        [string] $KeyFile = $null ,
        [switch] $UserAccount
    )

    if([string]::IsNullOrWhiteSpace($Password)) {
        $Password = $null;
    }

    if(![string]::IsNullOrWhiteSpace($KeyFile)) {
        if(!(Test-Path $KeyFile)) {
            throw [System.ArgumentException] "Could not find KeyFile at $KeyFile";
        }

        $KeyFile = (Resolve-Path $KeyFile).Path;
    }

    $key = New-Object NerdyMishka.KeePass.MasterKey;

    if($Password) {
        $pw = New-Object NerdyMishka.KeePass.MasterKeyPassword -ArgumentList $Password
        $key.Add($pw);
    }

    if(![string]::IsNullOrWhiteSpace($KeyFile)) {
        $kf = New-Object NerdyMishka.KeePass.MasterKeyfile -ArgumentList $KeyFile;
        $key.Add($kf);
    }

    if($UserAccount.ToBool()) {
        $ua = New-Object NerdyMishka.KeePass.MasterKeyUserAccount
        $key.Add($ua);
    }
  
    return ,$key;
}