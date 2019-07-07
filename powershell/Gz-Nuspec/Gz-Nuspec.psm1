if(!$PSScriptRoot) {
    $PSScriptRoot = $MyInovocation.PSScriptRoot
}

if(!$PSScriptRoot) {
    $PSScriptRoot = Split-Path $MyInovocation.MyCommand.Path
}

Get-Item "$PsScriptRoot\private\*.ps1" | ForEach-Object {
    . "$($_.FullName)"
}


Get-Item "$PsScriptRoot\public\*.ps1" | ForEach-Object {
    . "$($_.FullName)"
}

