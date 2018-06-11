function Read-KwehConfig() {
    Param(
        [Parameter(Position = 0)]
        [String] $Path 
    )

    if(-not (Test-Path $Path)) {
        throw [System.IO.FileNotFoundException] $Path 
    }

    $packageDir = (Resolve-Path $Path).Path
    if($packageDir.EndsWith("tools")) {
        $packageDir = Split-Path $packageDir
    }
    $cfg = Get-Content  $Path -Raw | ConvertFrom-Json
    $cfg | Add-Member NoteProperty PackageDir $packageDir
    return $cfg;   
}