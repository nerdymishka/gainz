function Update-WindowsTls() {
    $setting = [System.Net.SecurityProtocolType]::Tls -bor [System.Net.SecurityProtocolType]::Tls11 -bor [System.Net.SecurityProtocolType]::Tls12 
    [System.Net.ServicePointManager]::SecurityProtocol = $setting 
}