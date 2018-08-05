function Get-ChocolateyDownloadUri() {

    $uri = $null;
    $chocolateyVersion = $Env:ChocolateyVersion
    $chocolateyDownloadUrl = $Env:ChocolateyDownloadUrl

    if (![string]::IsNullOrEmpty($chocolateyVersion)){
        Write-Debug "Downloading specific version of Chocolatey: $chocolateyVersion"
        $uri = "https://chocolatey.org/api/v2/package/chocolatey/$chocolateyVersion"
        return $uri;
    }
      
      
    if (![string]::IsNullOrEmpty($chocolateyDownloadUrl)){
        Write-Debug "Downloading Chocolatey from : $chocolateyDownloadUrl"
        $uri = "$chocolateyDownloadUrl"

        return $uri;
    }

    if ([string]::IsNullOrWhiteSpace($uri)) {
        Write-Debug "Getting latest version of the Chocolatey package for download."
        $uri = 'https://chocolatey.org/api/v2/Packages()?$filter=((Id%20eq%20%27chocolatey%27)%20and%20(not%20IsPrerelease))%20and%20IsLatestVersion'
        [xml]$result = Get-WebRequestContentAsString $uri
        $uri = $result.feed.entry.content.src
    }

    return $uri;
}