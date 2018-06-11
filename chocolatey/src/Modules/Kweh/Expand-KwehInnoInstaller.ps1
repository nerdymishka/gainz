function Expand-KwehInnoInstaller() {
    Param(
        [String] $Path,
        [String] $Destination
    )

    $cmd = Get-Command innoextract -ErrorAction SilentlyContinue
    if(!$cmd) {
        Write-Error "Innoextract is installed installed on the path"
        Write-Error "run `"choco install innoextract -y`""
        Write-Error "verify that Innoextract is availbe in the path"
        return
    }

    if(!(Test-Path $Path)) {
        throw System.IO.FileNotFoundException $Path 
    }

    if(!(Test-Path $Destination)) {
        New-Item $Destination -ItemType Directory | Write-Debug
    }

    $innoextract = $cmd.Path
    & $innoextract -e $Path -d $Destination
    Start-Sleep 1
    return $LASTEXITCODE 
}

Set-Alias -Name Expand-InnoInstaller  Expand-KwehInnoInstaller 