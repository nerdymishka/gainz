$mod = Get-Module "Gainz-ProtectData" -EA SilentlyContinue
if(!$mod) {
    $mod = Import-Module "Gainz-ProtectData" -EA SilentlyContinue -PassThru
    if(!$mod) {
        Install-Module "Gainz-ProtectData" -Force
    }
}

$mod = Get-Module "Gainz-PasswordGenerator" -EA SilentlyContinue
if(!$mod) {
    $mod = Import-Module "Gainz-PasswordGenerator" -EA SilentlyContinue -PassThru
    if(!$mod) {
        Install-Module "Gainz-PasswordGenerator" -Force
    }
}


function New-SftpStandardUser() {
    Param(
        [Parameter(Position = 0)]
        [String] $Login,

        [String] $PublicKey,
        
        [securestring] $Password,
        
        [String] $Phrase
    )

    $pw = $null;
    if($Password -ne $null) {
        $pw = New-Object pscredential -ArgumentList "", $Password
        $pw = $pw.GetNetworkCredential().Password;
    }
    if(![string]::IsNullOrWhiteSpace($Phrase)) {
        $pw = $Phrase;
    }
    $group = $Env:SFTP_STANDARD_GROUP;

    if($pw) {
        Write-Debug "Creating Standard User with Password"

        useradd -p $(echo $pw | openssl passwd -1 -stdin) -G $group $Login -m | Write-Host
    } else {
        Write-Debug "Creating Standard User without Password"
        useradd -G $group $Login -m | Write-Host
    }

    chown "root:$Login" "/home/$Login" | Write-Host
    mkdir /home/$login/.ssh | Write-Host
    touch /home/$login/.ssh/authorized_keys 

    
    if(![string]::IsNullOrWhiteSpace($PublicKey)) {
        if($PublicKey -match "https:") {
            Write-Debug "curl $PublicKey"
            curl $PublicKey >> /home/$login/.ssh/authorized_keys | Write-Host
        } elseif(Test-Path $PublicKey) {
            Write-Debug "cat $PublicKey"
            cat $PublicKey >> /home/$login/.ssh/authorized_keys | Write-Host
        } else {
            Write-Debug "write PublicKey"
            $PublicKey >> /home/$login/.ssh/authorized_keys | Write-Host
        }
    }

    chown ${Login}:${Login} -R /home/$Login/.ssh | Write-Host
    sudo chmod 0700 /home/$Login/.ssh | Write-Host
    sudo chmod 0600 /home/$Login/.ssh/authorized_keys | Write-Host
}

function Get-GroupId() {
    Param(
        [String] $Group 
    )

    $info = getent group $Group
    if(!$info) {
        return $null;
    }
    $data = $info.Split(":");
    if($data.Length -lt 3) {
        return $null;
    }
    return $data[2]; 
}

function Get-UserId() {
    Param(
        [String] $User
    )
    $r =  id -u $User
    if($r -match "no such user") {
        return $null;
    }

    return $r;
}

$privateKeyBytes = $null 

function Get-SftpPrivateKey() {
    Param()

    # saved to disk
    if((Test-Path "/etc/ssh/sftp-mount-key")) {
        return [System.IO.File]::ReadAllBytes("/etc/ssh/sftp-mount-key");
    }

    if(![string]::IsNullOrWhiteSpace($Env:SFTP_MOUNT_KEY)) {
        $data = [System.Text.Encoding]::UTF8.GetBytes($Env:SFTP_MOUNT_KEY)
        [System.IO.File]::WriteAllBytes("/etc/ssh/sftp-mount-key", $data)
        [System.Environment]::SetEnvironmentVariable("SFTP_MOUNT_KEY", "")
        return $data;
    }

    if((Test-Path "/run/secrets/sftp-mount-key")) {
        $data = [System.IO.File]::ReadAllBytes("/run/secrets/sftp-mount-key");
        return $data;
    }

    $next = New-Password -Length 30
    $next = [System.Text.Encoding]::UTF8.GetBytes($next);
    [System.IO.File]::WriteAllBytes("/etc/ssh/sftp-mount-key",$next);
    chown root:root "/etc/ssh/sftp-mount-key";
    chmod 0660 "/etc/ssh/sftp-mount-key";

    return $next;
}

function Mount-AzureFileStoragePath() {
    Param(
        [Parameter(Position = 0, Mandatory = $true)]
        [String] $Login,

        [Parameter(Position = 1, Mandatory = $true)]
        [Alias("src")]
        [String] $StorageAccountPath,

        [Alias("dest")]
        [String] $MountPath, 

        [Alias("sa-name")]
        [String] $StorageAccountName = $Env:SFTP_AZURE_STORAGE,

        [Alias("sa-key")]
        [securestring] $StorageAccountKey = $Env:SFTP_AZURE_STORAGE_KEY 
    )

    Mount-AzureFileStoragePath -sa-name
    $group= $Env:SFTP_STANDARD_GROUP;
    $groupId = Get-GroupId -Group $group
    $id = Get-UserId -User $Login;

    if(!$groupId) {
        Write-Warning "Unable to get id for group $group"
        return;
    }

    if(!$id) {
        Write-Warning "Uable to get id for user $Login";
        return;
    }

    if([string]::IsNullOrWhiteSpace($StorageAccountName)) {
        Write-Warning "Storage Account Name is missing";
        return 
    }

    if([string]::IsNullOrWhiteSpace($StorageAccountKey)) {
        Write-Warning "Storage Account Key is missing";
        return 
    }

    if([String]::IsNullOrWhiteSpace($MountPath)) {
        $MountPath = Split-Path $StorageAccountPath -Leaf 
    }

    mkdir -p /home/$Login/$MountPath

    $cmd = ""
    $cmd += "mount -t cifs //$StorageAccountName.file.core.windows.net/$StorageAccountPath "
    $cmd += "/home/$Login/$MountPath "
    $cmd += "-o vers=3.0,username=$StorageAccountName,password=$StorageAccountKey,serverino,uid=$id,gid=$groupId"

    mount -t cifs //$StorageAccountName.file.core.windows.net/$StorageAccountPath /home/$Login/$MountPath \ 
        -o vers=3.0,username=$StorageAccountName,password=$StorageAccountKey,serverino,uid=$id,gid=$groupId

    
    Write-Host "mount $LASTEXITCODE"
    Write-AzureMountCommand $cmd 
}

function Write-AzureMountCommand() {
    Param(
        [Parameter(Position = 0)]
        [String] $Command 
    )

    $sshMounts = "/etc/ssh/azure-mounts" 
    if(!Test-Path $sshMounts) {
        "" | Out-File "/etc/ssh/azure-mounts" -Encoding "UTF8"
    }

    $content = Get-Content $sshMounts -Raw
    $bytes = Get-SftpPrivateKey
    $decrypted = "";
    if($content.Length -gt 0) {
        $decrypted = Unprotect-String $content -PrivateKey $bytes
    }

    $decrypted += "$line" + [Environment]::NewLine
    $content = Protect-String $decrypted -PrivateKey $bytes
    $content | Out-File $sshMounts -Encoding "UTF8" 
    [Array]::Clear($bytes, 0, $bytes.Length);
}

function Invoke-MountAzureStorageAccounts() {
    $content = Get-Content $sshMounts -Raw
    $bytes = Get-SftpPrivateKey
    $decrypted = "";
    if($content.Length -gt 0) {
        $decrypted = Unprotect-String $content -PrivateKey $bytes
        [Array]::Clear($bytes, 0, $bytes.Length);
        Invoke-Expression $decrypted;
    }

}

function Unprotect-AzureMountCommand() {
    $content = Get-Content $sshMounts -Raw
    $bytes = Get-SftpPrivateKey
    $decrypted = "";
    if($content.Length -gt 0) {
        $decrypted = Unprotect-String $content -PrivateKey $bytes
        [Array]::Clear($bytes, 0, $bytes.Length);
    }

    return $decrypted;
}


Export-ModuleMember -Function @(
    "Invoke-MountAzureStorageAccounts",
    "New-SftpStandardUser",
    "Mount-AzureFileStoragePath",
    "Unprotect-AzureMountCommand",
    "Get-SftpPrivateKey"
)