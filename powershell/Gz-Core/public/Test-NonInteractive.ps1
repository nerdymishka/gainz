$gzIsInteractive = $null

function Set-Interactive() {
    Param()

    if((Test-OsPlatform "Linux") -eq $True)
    {
        export DEBIAN_FRONTEND=
    }

    $gzIsInteractive = $true;
}

function Set-NonInteractive() {
    Param()

    if((Test-OsPlatform "Linux") -eq $True)
    {
        export DEBIAN_FRONTEND=noninteractive
    }
   
    $gzIsInteractive = $false;
}

function Test-NonInteractive() {
    if ([Environment]::UserInteractive) {
        if($null -ne $gzIsInteractive -and $gzIsInteractive -eq $false) {
            return $true;
        }

        if($Env:DEBIAN_FRONTEND -eq "noninteractive") {
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