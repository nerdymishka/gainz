
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


Export-ModuleMember -Function @(
    'Add-GzVisualStudioAlias',
    'Add-GzVisualStudioVersionAlias',
    "Get-GzMsBuildPath",
    "Get-GzVisualStudioBuildToolsPath",
    'Get-VisualStudioPath',
    "Get-GzVisualStudioVersion",
    "Get-GzVisualStudioTestConsolePath",
    "Invoke-GzMsBuild"
    'Invoke-GzVisualStudioBuild',
    'Invoke-GzVisualStudioTestConsole',
    'Read-GzVisualStudioSolution',
    'Remove-GzVisualStudioAlias'
) 