
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
    'Add-VisualStudioVersionAlias',
    "Get-MsBuildPath",
    'Get-VisualStudioPath',
    "Get-VisualStudioVersion",
    "Get-VisualStudioTestConsolePath",
    "Invoke-MsBuild"
    'Invoke-VisualStudioBuild',
    'Invoke-VisualStudioTestConsole',
    'Read-VisualStudioSolution'
) -Alias @(
    'Get-VsPath',
    'Get-VsTestPath',
    'Invoke-VsTest',
    'Invoke-VsBuild',
    'Read-VsSolution'
)