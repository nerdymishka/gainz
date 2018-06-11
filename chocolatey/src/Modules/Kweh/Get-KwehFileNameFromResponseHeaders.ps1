
$invalidFileCharacters = [System.IO.Path]::GetInvalidFileNameChars()
$invalidFileCharacters = [System.Text.RegularExpressions.Regex]::Escape($invalidFileCharacters)
$invalidFileCharacters = "[$invalidFileCharacters\=\;]"

function Get-KwehFileNameFromResponseHeaders() {
    Param(
        [Parameter(Position = 0)]
        [System.Net.WebResponse] $WebResponse,
        
        [Parameter(Position = 1)]
        [Uri] $RequestUri 
    )

    if($WebResponse -eq $null) {
        Write-Host "WebResponse Empty"
        return $null;
    }
    
    [string] $fileName 
    [Regex] $invalidCharactersTest =  New-Object System.Text.RegularExpressions.Regex($invalidFileCharacters);
    [string]$disposition = $WebResponse.Headers['Content-Disposition']
    [string]$location = $WebResponse.Headers['Location']
  
    # start with content-disposition header
    if (![string]::IsNullOrWhiteSpace($disposition)) {
        $match = 'filename='
        $index = $header.LastIndexOf($match, [StringComparison]::OrdinalIgnoreCase)
    
        if ($index -gt -1) {
            $fileName = $header.Substring($index + $fileHeaderName.Length).Replace('"', '')
        }
    }
   
    if ($invalidCharactersTest.IsMatch($fileName)) { 
        $fileName = $null
    } elseif(![string]::IsNullOrWhiteSpace($fileName)) {
        return $fileName.Trim()
    }
  

    if ([string]::IsNullOrWhiteSpace($fileName)) {
        if (![string]::IsNullOrWhiteSpace($location)) {
            Write-Debug "'Location' header containers filename."
            $fileName = [System.IO.Path]::GetFileName($location)
        }
    }

    if ($invalidCharactersTest.IsMatch($fileName)) { 
        $fileName = $null
    } elseif(![string]::IsNullOrWhiteSpace($fileName)) {
        return $fileName.Trim()
    }
  
    if ([string]::IsNullOrWhiteSpace($fileName)) {
        $responseUrl = $WebResponse.ResponseUri.ToString()
  
        if (!$responseUrl.Contains('?')) {
            Write-Debug "ResponseUri contains filename. '$responseUrl'"
            $fileName = [System.IO.Path]::GetFileName($responseUrl)
        }
    }
    
   
    if ($invalidCharactersTest.IsMatch($fileName)) { 
        $fileName = $null
    } elseif(![string]::IsNullOrWhiteSpace($fileName)) {
        return $fileName.Trim()
    }
  
   
    if ([string]::IsNullOrWhiteSpace($fileName) -and $RequestUri) {
        $requestUrl = $RequestUri.ToString()
        $extension = [System.IO.Path]::GetExtension($requestUrl)
        
        if (!$requestUrl.Contains('?') -and ![string]::IsNullOrWhiteSpace($extension)) {
            Write-Debug "RequestUrl contains filename. ' $requestUrl'" 
            $fileName = [System.IO.Path]::GetFileName($requestUrl)
        }
    }

   

    if ($invalidCharactersTest.IsMatch($fileName)) { $fileName = $null }
    if($fileName) {
        return $fileName.Trim()
    }
    return $fileName  
}

Set-Alias -Name Get-FileNameFromResponseHeaders -Value Get-KwehFileNameFromResponseHeaders