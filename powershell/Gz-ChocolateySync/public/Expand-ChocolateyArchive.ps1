function Expand-ChocolateyArchive() {
    Param(
        [Parameter(Position = 0)]
        [String] $Path,

        [Parameter(Position = 1)]
        [String] $Destination,

        [Switch] $Force 
    )

    if((Get-Command Expand-Archive -ErrorAction SilentlyContinue)) {
        Write-Debug "Extracting $Path to $Destination..."
        Expand-Archive -Path $Path -DestinationPath $Destination -Force:$Force | Write-Debug
        return;
    }

    # Determine unzipping method
    # 7zip is the most compatible so use it by default
    $7zaExe = Join-Path $Destination '7za.exe'
    $unzipMethod = '7zip'
    $useWindowsCompression = $env:chocolateyUseWindowsCompression
    if (!$useWindowsCompression -and $useWindowsCompression -eq 'true') {
        Write-Debug 'Using built-in compression to unzip'
        $unzipMethod = 'builtin'
    } elseif (-not (Test-Path ($7zaExe))) {
        Write-Debug "Downloading 7-Zip commandline tool prior to extraction."
        # download 7zip
        Get-WebRequestContentAsFile -Uri 'https://chocolatey.org/7za.exe' -Destination "$7zaExe"
    }

    # unzip the package
    Write-Debug "Extracting $Path to $Destination..."
    if ($unzipMethod -eq '7zip') {
        $params = "x -o`"$Destination`" -bd -y `"$Path`""
  
        # use more robust Process as compared to Start-Process -Wait (which doesn't
        # wait for the process to finish in PowerShell v3)
        $process = New-Object System.Diagnostics.Process
        $process.StartInfo = New-Object System.Diagnostics.ProcessStartInfo($7zaExe, $params)
        $process.StartInfo.RedirectStandardOutput = $true
        $process.StartInfo.UseShellExecute = $false
        $process.StartInfo.WindowStyle = [System.Diagnostics.ProcessWindowStyle]::Hidden
        $process.Start() | Out-Null
        $process.BeginOutputReadLine()
        $process.WaitForExit()
        $exitCode = $process.ExitCode
        $process.Dispose()

        $errorMessage = "Unable to unzip package using 7zip. Perhaps try setting `$env:chocolateyUseWindowsCompression = 'true' and call install again. Error:"
        switch ($exitCode) {
            0 { break }
            1 { throw "$errorMessage Some files could not be extracted" }
            2 { throw "$errorMessage 7-Zip encountered a fatal error while extracting the files" }
            7 { throw "$errorMessage 7-Zip command line error" }
            8 { throw "$errorMessage 7-Zip out of memory" }
            255 { throw "$errorMessage Extraction cancelled by the user" }
            default { throw "$errorMessage 7-Zip signalled an unknown error (code $exitCode)" }
        } 
    } else {
        $shellApplication = new-object -com shell.application
        $zipPackage = $shellApplication.NameSpace($Path)
        $destinationFolder = $shellApplication.NameSpace($Destination)
        $destinationFolder.CopyHere($zipPackage.Items(),0x10)
    }
}