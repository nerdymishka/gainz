Param(
    [Parameter(Position = 0)]
    [String] $Json 
)

<#
{
    "label": "citco",
    "login": "some_sftp_user",
    "storageAccount: {
        "name": "name",
        "key": "key",
        "path": "{container}/path/to/point"
    },
    mountPath: "folder in user's home directory"
}

ssh user2@host2 "pwsh -Command '/opt/powershell/posh-sftp/New-AzureSftpStandarUser.ps1 -Json `"$Json`"'"
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
    StorageAccountPath = $parameters.storageAccount.Path
    MountPath = $parameters.MountPath 
    StorageAccountName = $parameters.storageAccount.Name 
    StorageAccountKey = $parameters.storageAccount.Key
}

Mount-AzureFileStoragePath @argz 