Param(
    [String] $Json
)

<#
{
    "mount": false,
    "privateKey": base64
    "data": base64
}

ssh user2@host2 "pwsh -Command '/opt/powershell/posh-sftp/Update-AzureMountData.ps1 -Json `"$Json`"'"

#>

#TODO: read current mount data and umount files

$message = ConvertFrom-Json $json 

if(!$message.privateKey) {
    Write-Warning "Missing private Key";
}
if(!$message.data) {
    Write-Warning "Missing data";
}

Import-Module "$PSScriptRoot/posh-sftp.psm1" -Force

# should be utf8
$bytes = [Convert]::FromBase64String($message.privateKey)
$exists = Test-Path "/etc/ssh/sftp-mount-key"
[System.IO.File]::WriteAllBytes("/etc/ssh/sftp-mount-key", $bytes);

if(!$exists) {
    chown root:root "/etc/ssh/sftp-mount-key"
    chmod 0660 "/etc/ssh/sftp-mount-key"  
}

$content = $message.data 
[System.IO.File]::WriteAllText("/etc/ssh/azure-mounts", $content, [System.Text.Encoding]::UTF8) 

$decrypted = $null;
if($message.mount) {
    $decrypted = Unprotect-String $content -PrivateKey $bytes
}

[Array]::Clear($bytes, 0, $bytes.Length)
if($decrypted) {
    Start-Transcript -Path "/var/log/azure-mount.log"
    Invoke-Expression $decrypted
    Stop-Transcript
}

return 0;