function Test-Interactive() {


    if($null -ne $Env:GAINZ_INTERACTIVE) {
        return $Env:GAINZ_INTERACTIVE -eq "1"
    }

    if(![Environment]::UserInteractive) {
        $Env:GAINZ_INTERACTIVE = "0"
        return $false;
    }

    $args = [Environment]::GetCommandLineArgs()
    foreach($arg in $args) {
        if($arg -match "NonInteractive") {
            $Env:GAINZ_INTERACTIVE = "0";
            return $false;
        }
    }

    $Env:GAINZ_INTERACTIVE = "1"
    return $true;
}