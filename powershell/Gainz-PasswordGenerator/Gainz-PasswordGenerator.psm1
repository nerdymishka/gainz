
if(!$PSScriptRoot) {
    $PSScriptRoot = $MyInovocation.PSScriptRoot
}

if(!$PSScriptRoot) {
    $PSScriptRoot = Split-Path $MyInovocation.MyCommand.Path
}

Get-Item "$PsScriptRoot\public\*.ps1" | ForEach-Object {
    . "$($_.FullName)"
}


Export-ModuleMember -Function @(
    'New-Password'
)