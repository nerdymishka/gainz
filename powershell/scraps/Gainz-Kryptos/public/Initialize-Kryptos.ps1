function Initialize-Kryptos() {
    Param(
        
    )

    $kryptos = Get-Command kryptos -EA SilentlyContinue
    if(!$kryptos) {
        Write-Warning "kryptos is not installed or is not on the path. try running: choco install kryptos -yf"
        return;
    }

    & kryptos cert new
}