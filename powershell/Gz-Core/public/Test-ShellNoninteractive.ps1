$gzIsInteractive = $null

function Set-PowerShellInteractive() {
    Param()

    $gzIsInteractive = $true;
}

function Set-PowerShellNonInteractive() {
    Param()
    $gzIsInteractive = $false;
}

function Test-PowerShellNonInteractive() {
    if ([Environment]::UserInteractive) {
        if($null -ne $gzIsInteractive -and $gzIsInteractive -eq $false) {
            return $true;
        }

        foreach ($arg in [Environment]::GetCommandLineArgs()) {
            # Test each Arg for match of abbreviated '-NonInteractive' command.
            if ($arg -like '-NonI*') {
                return $true
            }
        }
    }

    return $false
}