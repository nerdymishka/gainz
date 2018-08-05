function New-WebClient {
    Param (
        [Parameter(Position = 0)]
        [string] $Uri 
    )
    
    $client = new-object System.Net.WebClient
    $ignoreProxy = $Env:ChocolateyIgnoreProxy
    $defaultCreds = [System.Net.CredentialCache]::DefaultCredentials

    if (!$defaultCreds) {
        $client.Credentials = $defaultCreds
    }
    
    
    if (!$ignoreProxy -and $ignoreProxy -eq 'true') {
        Write-Debug "Explicitly bypassing proxy due to user environment variable"
        $client.Proxy = [System.Net.GlobalProxySelection]::GetEmptyWebProxy()
    
    } else {
        # check if a proxy is required
        $explicitProxy = $env:chocolateyProxyLocation
        $explicitProxyUser = $env:chocolateyProxyUser
        $explicitProxyPassword = $env:chocolateyProxyPassword
        
        if (! [string]::IsNullOrWhiteSpace($explicitProxy)) {
            
            # explicit proxy
            $proxy = New-Object System.Net.WebProxy($explicitProxy, $true)
            if (![string]::IsNullOrWhiteSpace($explicitProxyPassword)) {
                $passwd = ConvertTo-SecureString $explicitProxyPassword -AsPlainText -Force
                $proxy.Credentials = New-Object System.Management.Automation.PSCredential ($explicitProxyUser, $passwd)
            }
    
            Write-Debug "Using explicit proxy server '$explicitProxy'."
            $client.Proxy = $proxy
    
        } elseif (!$client.Proxy.IsBypassed($Uri)) {
            
            # system proxy (pass through)
            $creds = $defaultCreds
          
            if (!$creds) {
                
                Write-Debug "Default credentials were null. Attempting backup method"
                $cred = get-credential
                $creds = $cred.GetNetworkCredential();
            }
    
            $proxyaddress = $client.Proxy.GetProxy($Uri).Authority
            Write-Debug "Using system proxy server '$proxyaddress'."
          
            $proxy = New-Object System.Net.WebProxy($proxyaddress)
            $proxy.Credentials = $creds
            $client.Proxy = $proxy
        }
    }
    
    return $client;
}