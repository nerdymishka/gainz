if(!$PsScriptRoot) {
    $PsScriptRoot = $MyInovocation.PSScriptRoot
}

if(!$PsScriptRoot) {
    $PsScriptRoot = Split-Path $MyInovocation.MyCommand.Path
}


Get-Item "$PsScriptRoot\*.ps1" | ForEach-Object {
     if($_.NAme -eq "Kweh-Packager.ps1") {
         return;
     }
     . "$($_.FullName)"
}