function Expand-KwehMicrosoftInstaller() {
    Param(
        [Parameter(Position = 0)]
        [String] $Path,

        [Parameter(Position = 1)]
        [String] $Destination
    )

    if(-not (Test-Path $Path)) {
        throw [System.IO.FileNotFoundException] $Path 
    }

    if(-not (Test-Path $Destination)) {
        New-Item $Destination -ItemType Directory
    }

    $Path = (Resolve-Path $Path).Path
    $Destination = (Resolve-Path $Destination).Path

    Write-Host "expand -msi"
    Write-Host "$path"
    Write-Host "msiexec.exe /a "$PATH" TARGETDIR=`"$Destination`" /qn"

    msiexec.exe /a "$Path" TARGETDIR="$Destination" /qn 
    Start-Sleep 1

    return $LASTEXITCODE
}

Set-Alias -Name Expand-MicrosoftInstaller -Value Expand-KwehMicrosoftInstaller 