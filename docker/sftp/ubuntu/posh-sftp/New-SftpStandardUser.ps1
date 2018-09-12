Param(
    [Parameter(Position = 0)]
    [String] $Json 
)

<#
{
    "login": "some_sftp_user",
    "publicKey": "https|file/path|keyData",
    "phrase": "base64/utf8 string"
}
the user is added to the standard_sftp_users group, which is jailed.

ssh user2@host2 "pwsh -Command '/opt/powershell/posh-sftp/New-SftpStandarUser.ps1 -Json `"$Json`"'"

#>

if([string]::IsNullOrWhiteSpace($Json)) {
    Write-Warning "Json is empty";
    return;
}

Import-Module "$PSScriptRoot/posh-sftp.psm1" -Force
$parameters = ConvertFrom-Json $json 
$pw = $null;
if($parameters.Phrase) {
    $pw = [Convert]::FromBase64String($parameters.Phrase);
    $pw = [System.Text.Encoding]::UTF8.GetString($pw);
}

$argz = @{
    Login = $parameters.Login
    Phrase = $pw 
    PublicKey = $parameters.PublicKey 
}

New-SftpStandardUser @argz  