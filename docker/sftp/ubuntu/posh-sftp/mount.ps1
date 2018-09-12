
if(Test-Path "/etc/ssh/azure-mount") {
    $content = Get-Content "/etc/ssh/azure-mount" -Raw;

    if($content.Length -lt 10) {
        return 0;
    } 

    try {
        Start-Transcript -Path "/var/log/azure-mount.log" -NoClobber

        Import-Module "$PSScriptRoot/posh-sftp.psm1" -Force

        Invoke-MountAzureStorageAccounts 

        Stop-Transcript  
    } catch  {
        $_.Exception.ToString() | Add-Content -Path "/var/log/azure-mount.log" -Force
        return 0;
    }

    return 0;
}