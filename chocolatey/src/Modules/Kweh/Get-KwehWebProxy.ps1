function Get-KwehWebProxy() {
    [CmdletBinding()]
    Param()

    $proxyLocation = $env:chocolateyProxyLocation

    if($proxyLocation -ne $NULL) {
        $proxy = New-Object System.Net.WebProxy($proxyLocation, $true)

        $proxyUser = $env:chocolateyProxyUser
        $proxyPassword = $env:chocolateyProxyPassword
        $proxyBypassList = $env:chocolateyProxyBypassList
        $proxyBypassOnLocal = $env:chocolateyProxyBypassOnLocal

        
        
        if ($proxyPassword -ne $null) {
            $passwd = ConvertTo-SecureString $proxyPassword -AsPlainText -Force
	        $proxy.Credentials = New-Object System.Management.Automation.PSCredential ($proxyUser, $passwd)
	    }
    
        if ($proxyBypassList -ne $null -and $explicitProxyBypassList -ne '') {
            $proxy.BypassList =  $proxyBypassList.Split(',', [System.StringSplitOptions]::RemoveEmptyEntries)
        }
        
        if ($proxyBypassOnLocal -eq 'true') { $proxy.BypassProxyOnLocal = $true; }
             
        Write-Debug "Proxy server '$explicitProxy'."

        return $proxy;
    }

    $WebClient = New-Object System.Net.WebClient  
    $networkCredential = [System.Net.CredentialCache]::DefaultCredentials
    if($networkCredential) {
        $WebClient.Credentials = $networkCredential
    }
   
    if ($WebClient -and $WebClient.Proxy -and !$WebClient.Proxy.IsBypassed($Uri)) {
	  # system proxy (pass through)
        
        if ($networkCredential -eq $null) {
            Write-Debug "Default credentials were null. Attempting backup method"
            
            $credential = get-credential
            $networkCredential = $credential.GetNetworkCredential();
        }
        
        $proxyaddress = $webclient.Proxy.GetProxy($url).Authority

        $proxy = New-Object System.Net.WebProxy($proxyaddress)
        $proxy.Credentials = $creds
        $proxy.BypassProxyOnLocal = $true

        Write-Debug "Proxy server '$proxyaddress'."

        return $proxy;
    }

    return $null
}

Set-Alias -Name Get-WebProxy -Value Get-KwehWebProxy