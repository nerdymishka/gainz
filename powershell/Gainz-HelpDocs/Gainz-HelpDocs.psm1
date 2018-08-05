if(!$PSScriptRoot) {
    $PSScriptRoot = $MyInovocation.PSScriptRoot
}

if(!$PSScriptRoot) {
    $PSScriptRoot = Split-Path $MyInovocation.MyCommand.Path
}

Get-Item "$PSScriptRoot\public\*.ps1" | ForEach-Object {
    . "$($_.FullName)"
}

$functions  = @(
    "Read-HelpExample",
    "Read-ModuleHelp",
    "Write-CmdletAlias",
    "Write-CmdletDescription",
    "Write-CmdletDoc",
    "Write-CmdletExample",
    "Write-CmdletFrontMatter",
    "Write-CmdletInput",
    "Write-CmdletLink",
    "Write-CmdletName",
    "Write-CmdletNote",
    "Write-CmdletOutput",
    "Write-CmdletParameter",
    "Write-CmdletSynopsis",
    "Write-CmdletSyntax",
    "Write-EscapedMarkdownString",
    "Write-ModuleHelpDocs"
) 


Export-ModuleMember -Function $functions -Alias @('Write-Help')
