if(!$PSScriptRoot) {
    $PSScriptRoot = $MyInovocation.PSScriptRoot
}

if(!$PSScriptRoot) {
    $PSScriptRoot = Split-Path $MyInovocation.MyCommand.Path
}

#Get-Item "$PSScriptRoot\private\*.ps1" | ForEach-Object {
#    . "$($_.FullName)"
#}

Get-Item "$PSScriptRoot\public\*.ps1" | ForEach-Object {
    . "$($_.FullName)"
}

Export-ModuleMember -Function @(
    'Select-Html',
    'New-HtmlDocument'
)