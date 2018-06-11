
function New-KwehWebRequest() {
    Param(
        [Parameter(Position=0)]
        [Uri] $Uri,
    
        [Parameter(Position=1, Mandatory=$false)]
        [string] $UserAgent = 'kweh command line',


        [string] $Method = "GET",
        
        [Parameter()]
        [Hashtable] $Options 
    )

    if(!$URI -or [string]::IsNullOrWhiteSpace($Uri.ToString())) {
        throw [System.ArgumentException] "Uri is empty"
    }

    $Options = if(!$Options) { @{Headers = @{}} } else { $Options }

    $accept = if($options.Accept) { $options.Accept } else { "*/*" }
    $timeout = if($Env:ChocolateyRequestTimeout){ $Env:ChocolateyRequestTimeout } else { 30000 }
    $responseTimeout = if($Env:ChocolateyRepsonseTimeout) {$env:chocolateyResponseTimeout} else { $null }
    $decompression = [System.Net.DecompressionMethods]::GZip -bor [System.Net.DecompressionMethods]::Deflate
    $redirects = if($Options.MaximumAutomaticRedirections) { $Options.MaximumAutomaticRedirections } else { 20 }

    $httpRequest = [System.Net.HttpWebRequest]::Create($Uri);

    $httpRequest.Method = $Method 
    $httpRequest.Accept = $accept
    $httpRequest.AllowAutoRedirect = $true
    $httpRequest.MaximumAutomaticRedirections = $redirects
    $httpRequest.AutomaticDecompression = $decompression
    $httpRequest.Timeout = $timeout
    if($responseTimeout) { $httpRequest.ReadWriteTimeout = $responseTimeout }
    
    $httpRequest.CookieContainer = New-Object System.Net.CookieContainer
  
    if(![string]::IsNullOrWhiteSpace($UserAgent)) {
        $httpRequest.UserAgent = $UserAgent
    }
  
    if ($Options.Headers -and $Options.Headers.Count -gt 0) {

        foreach ($item in $options.Headers.GetEnumerator()) {
            
            switch ($item.Key) {
                'Accept' {$httpRequest.Accept = $item.Value}
                'Cookie' {$httpRequest.CookieContainer.SetCookies($uri, $item.Value)}
                'Referer' {$httpRequest.Referer = $item.Value}
                'User-Agent' {$httpRequest.UserAgent = $item.Value}
                Default {
                    $httpRequest.Headers.Add($item.Key, $item.Value)
                }
            }
        }
    }

    $defaultCredentials = [System.Net.CredentialCache]::DefaultCredentials
    if ($defaultCredentials -ne $null) {
        $httpRequest.Credentials = $defaultCredentials
    }

    return $httpRequest
}