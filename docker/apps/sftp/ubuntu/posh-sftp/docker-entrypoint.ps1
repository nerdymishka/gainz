
chmod 0760 /etc/ssh/sshd_config 
chmod 0760 /etc/ssh/ssh_config 

if([string]::IsNullOrWhiteSpace($Env:SFTP_ADMIN_KEY_URI)) {
    Write-Error "Public key uri (SFTP_ADMIN_KEY_URI) was not specified"
    return 1;
}
if(!(Test-Path "/etc/ssh/ssh_host_rsa_key")) {
    ssh-keygen -t rsa -b 4096 -N '""' -f /etc/ssh/ssh_host_rsa_key
}

if(!(Test-Path "/etc/ssh/ssh_host_ed25519_key")) {
    ssh-keygen -t ed25519 -f /etc/ssh/ssh_host_ed25519_key -N '""'
}



if(!(Test-Path "/etc/ssh/ssh_host_ecdsa_key")) {
    ssh-keygen -t ecdsa -b 521 -f /etc/ssh/ssh_host_ecdsa_key -N '""'
}

$dir = Test-Path /home/$($Env:SFTP_ADMIN)

if(!$dir) {
    $g = $Env:SFTP_STANDARD_GROUP
    $u = $Env:SFTP_ADMIN
    groupadd -g 9000 $g 
    groupadd -g 3000 ssh_users
    useradd $u -m -s /bin/bash 
    usermod -aG sudo $u
    usermod -aG root $u
    usermod -aG ssh_users $u 
    if([string]::IsNullOrWhiteSpace($Env:SFTP_SUDO_PHRASE)) {
        passwd -d $u
    } else {
        usermod --password $(echo $($Env:SFTP_SUDO_PHRASE) | openssl passwd -1 -stdin) $u 
    }
}

if(!(Test-Path "/home/$($Env:SFTP_ADMIN)/.ssh")) {
    mkdir -p "/home/$($Env:SFTP_ADMIN)/.ssh"
    touch "/home/$($Env:SFTP_ADMIN)/.ssh/authorized_keys"


    if( Test-Path "$($Env:SFTP_ADMIN_KEY_URI)") {
        cat "$($Env:SFTP_ADMIN_KEY_URI)" >> "/home/$($Env:SFTP_ADMIN)/.ssh/authorized_keys"
    } else {
        if("$($Env:SFTP_ADMIN_KEY_URI)" -match "https:") {
            write-host "curling"
            curl "$($Env:SFTP_ADMIN_KEY_URI)" >> "/home/$($Env:SFTP_ADMIN)/.ssh/authorized_keys"
        }
    }

    $user = $Env:SFTP_ADMIN
    chown ${user}:${user} -R /home/$($user)
    chown ${user}:${user} -R /home/$($user)/.ssh
    chmod 0700 /home/$user/.ssh
    chmod 0600 /home/$user/.ssh/authorized_keys
}

Import-Module "/opt/powershell/posh-sftp/posh-sftp.psm1"


$argz = @();
$cmd = $args[0];
for($i = 1; $i -lt $args.Length; $i++) {
    $p = $args[$i]
    $argz += $p;
}

& $cmd @argz