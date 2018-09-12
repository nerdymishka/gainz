Param(
    [Parameter(Position = 0)]
    [String] $Json 
)

<#
{
    "login": "some_sftp_user",
    "publicKey": "https|file/path|keyData",
    "phrase": "base64/utf8 string"
    "storageAccount: {
        "name": "name",
        "key": "key",
        "path": "{container}/path/to/point"
    },
    mountPath: "folder in user's home directory"
}
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

$argz = @{
    Login = $parameters.Login 
    StorageAccountPath = $parameters.storageAccount.Path
    MountPath = $parameters.MountPath 
    $StorageAccountName = $parameters.StorageAccount.Name 
    $StorageAccountKey = $parameters.StorageAccount.Key
}

Mount-AzureFileStoragePath @argz 