function Invoke-KwehWebRequest() {
    Param(
        [Uri] $Uri,

        [System.Net.WebProxy] $Proxy,

        [string] $Method,

        [string] $UserAgent = "kweh command line",

        [Hashtable] $Options,

        [switch] $HeadersOnly,

        [string] $OutFile 
    )

    $Options = if($Options) { $Options } else { @{Headers = @{}} }

    $httpRequest = New-KwehWebRequest -Uri $Uri -UserAgent $UserAgent -Options $Options

    $Proxy = if($Proxy) { $Proxy } else { Get-KwehWebProxy }
    if($Proxy) {
        $httpRequest.Proxy = $Proxy
    } 

    $httpResponse = $null;
    try {
        $httpResponse = $httpRequest.GetResponse();

        if($statusCode -eq 401 -or $statusCode -eq 403 -or $statusCode -eq 404) {
            $Env:ChocolateyExitCode = $statusCode
            throw "HttpRequest failed with $statusCode. Remote file either doesn't exist, is unauthorized, or is forbidden for '$Uri'."
        }
    } catch {
        if($httpRequest -ne $null) {
            $httpRequest.ServicePoint.MaxIdleTime = 0
            $httpRequest.Abort();
            
            Remove-Variable httpRequest 
            Start-Sleep 1
            [System.GC]::Collect()
            
        }  
        Write-Error "$URI failed ";
        throw $_.Exception
    } 

    $statusCode = $httpResponse.StatusCode
    if($HeadersOnly.ToBool()) {
        $responseHeaders = @{}
    
        foreach ($key in $httpResponse.Headers) {
            $value = $httpResponse.Headers[$key];
            if ($value) {
                $responseHeaders.Add("$key","$value")
            }
        }

    
        return $responseHeaders;
    }
   
    if([string]::IsNullOrWhiteSpace($OutFile)) {
        $OutFile = $null
    } else {
        $OutFile = $OutFile.Trim()
    }
    
    if($OutFile[0] -eq '.' -or $OutFile[0] -eq '~' -and $OutFile[1] -eq '/' -or $OutFile[1] -eq '\') {
        $OutFile = Join-Path (Get-Location -PSProvider "FileSystem") ($OutFile.Substring(2))
    }
    Write-Host $OutFile 
    # Leaf node only
    if($OutFile -and !(Split-Path $OutFile)) {
        Write-Host "leaf node"
        $OutFile = Join-Path (Get-Location -PSProvider "FileSystem") $OutFile
    }

   
    if($OutFile -and (Test-Path -PathType "Container" $OutFile))
    {
        $parentDirectory = $OutFile
        $OutFile = Get-FileNameFromResponseHeaders -WebResponse $httpResponse -RequestUri $Uri  
        
        if([string]::IsNullOrWhiteSpace($OutFile)) {
            $OutFile = Read-Host -Prompt "Enter a name for the file"
        }
        
        $OutFile = Join-Path $parentDirectory ($OutFile.Trim())
       
    }

     Write-Debug "Out-File $outFile"

    if($statusCode -eq 200 -and $OutFile) {
        $length = $httpResponse.ContentLength 
        $stream = $httpResponse.GetResponseStream()

        $parentDirectory = [System.IO.Path]::GetDirectoryName($OutFile)
        if(-not (Test-Path $parentDirectory)) {
            New-Item $parentDirectory -ItemType Directory -Force | Write-Debug
        }

        $writer = New-Object System.IO.FileStream $OutFile, "Create"

        [byte[]]$buffer = new-object byte[] 1048576
        [long]$bytesWritten = 0;
        [long]$bytesRead = 0; 
        [long]$i = 0;

        $lengthFormatted = Format-FileSize $length;

        $eap = $ErrorActionPreference
        $ErrorActionPreference = 'Stop'

        try {
            do {
                $bytesRead = $stream.Read($buffer, 0, $buffer.Length);
                $writer.Write($buffer, 0, $bytesRead);
                

                $bytesWritten += $bytesRead
                $totalFormatted = Format-FileSize $bytesWritten
                
                if($length -gt 0 -and ++$i % 10 -eq 0) {
                    $percentComplete = [Math]::Truncate(($bytesWritten/$length)*100)
                    Write-Progress "Downloading $uri to $OutFile" "Saving $totalFormatted of $lengthFormatted" `
                        -Id 0 -PercentComplete $percentComplete
                }

                if ($bytesWritten -eq $length -and $bytesRead -eq 0) {
                    Write-Progress "Completed download of $uri." "Completed download of $OutFile ($lengthFormatted)."`
                        -Id 0 -Completed -PercentComplete 100
                }
            } while($bytesRead -gt 0)

            Start-Sleep 1
            return Get-Item $OutFile 
        } finally {
            $stream.Dispose()
            $writer.Flush()
            $writer.Dispose()
            $ErrorActionPreference = $eap  
        }
    }

    return $httpResponse;
}