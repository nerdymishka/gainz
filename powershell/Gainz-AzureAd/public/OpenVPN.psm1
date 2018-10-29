function Update-OpenVPNConfig() {
    Param(
        [String] $MountPath
    )

    $file = "$MountPath/data/aad-users.json"
    $data = Get-Content $file -Raw | ConvertFrom-Json
    $employees = $data | Where-Object {$_.membershipStatus -eq "employee" -and $_.isEnabled -eq $true }

    foreach($employee in  $employees)
    {
        if($employee.email)
        {
            New-UserConfigurationFolder -MountPath $MountPath -User ($employee.email)
            New-OpenVPNConfig -MountPath $MountPath -User ($employee.email)
        }
    }
}


function New-UserConfigurationFolder() {
    Param(
        [String] $MountPath,
        [String] $User
    )

    $folderName = $User
    if($User.Contains("@")) {
        $folderName = $user.Substring(0, $user.IndexOf("@"))
    }

    $userland = "$MountPath/data/configuration_files/userland"
    $dir = "$userland/$folderName"
    if(!(Test-Path $dir)) {
        New-Item $dir -ItemType Directory
    }
}


function New-OpenVPNConfig() {
    Param(
        [String] $User,
        [String] $MountPath
    )

    $cert = "/opt/easy-rsa/pki/issued/$User.crt"
    $key = "/opt/easy-rsa/pki/private/$User.key"

    if(!(Test-Path $key)) {
        sudo ./easyrsa gen-req $User nopass
    }

    if(!(Test-Path $cert))
    {
        sudo ./easyrsa sign-req client $cert 
    }

    $ovpn = "/opt/easy-rsa/client-configs/files/ul1-$User.ovpn"
    
    if(!(Test-Path $ovpn))
    {
        $ca = "/opt/easy-rsa/pki/ca.crt"
        $ta = "/opt/easy-rsa/ta.key"
        $config = "/opt/easy-rsa/client-configs/base.config"

        $out = Get-Content $config -Raw 

        $out += "`n<ca>"
        $out += (Get-Content $ca -Raw)
        $out += "`n</ca>"
        
        $out += "`n<cert>"
        $out += (Get-Content $cert -Raw)
        $out += "`n</cert>"
        
        $out += "`n<key>"
        $out += (Get-Content $key -Raw)
        $out += "`n</key>"
        
        $out += "`n<tls-auth>"
        $out += (Get-Content $ta -Raw)
        $out += "`n</tls-auth>"

        $out | Out-File "/opt/easy-rsa/client-configs/files/ul1-$User.ovpn" -Encoding "UTF8"
    }

    if($Env:USERLAND_KEY) {
       $folderName = $User
       if($User.Contains("@")) {
            $folderName = $user.Substring(0, $user.IndexOf("@"))
       }
       $encrypted = Protect-String (Get-Content $ovpn -Raw) -PrivateKey $Env:USERLAND_KEY 
       $encrypted | Out-File "$MountPath/configuration_files/userland/$folderName/ul1-$User.ovpn"
    }
}