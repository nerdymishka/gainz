
function Enable-GzWindowsTls() {
    Param(
        [ValidateSet("1.0", "1.1", "1.2", "1.3")]
        [Parameter(Position = 1)]
        [String] $Version,

        [Switch] $ServerOnly,

        [Switch] $ClientOnly
    )

    if($null -eq $Version)
    {
        $Version = "1.2"
    }

    $base = "HKLM:\SYSTEM\CurrentControlSet\Control\SecurityProviders\SCHANNEL\Protocols\TLS $Version"
    $server = "$base\Server"
    $client = "$base\Client"

    if(!$ClientOnly.ToBool())
    {
        New-Item $server -Force | Out-Null
        New-ItemProperty -path $server  -name 'Enabled' -value '1' -PropertyType 'DWord' -Force | Out-Null
        New-ItemProperty -path $server -name 'DisabledByDefault' -value 0 -PropertyType 'DWord' -Force | Out-Null
    }
   

    if(!$ServerOnly.ToBool())
    {
        New-Item $client -Force | Out-Null
        New-ItemProperty -path $client -name 'Enabled' -value '1' -PropertyType 'DWord' -Force | Out-Null
        New-ItemProperty -path $client -name 'DisabledByDefault' -value 0 -PropertyType 'DWord' -Force | Out-Null
    }

    return $true;
}


function Enable-GzWindowsStrongCrypto() {
    Param(

    )

    New-ItemProperty -path 'HKLM:\SOFTWARE\Microsoft\.NetFramework\v4.0.30319' -name 'SchUseStrongCrypto' -value '1' -PropertyType 'DWord' -Force | Out-Null
}



 