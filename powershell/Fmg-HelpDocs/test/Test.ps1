if(!$PSScriptRoot) {
    $PSScriptRoot = $MyInovocation.PSScriptRoot
}

if(!$PSScriptRoot) {
    $PSScriptRoot = Split-Path $MyInovocation.MyCommand.Path
}



Get-Item "$PSScriptRoot\..\public\*.ps1" | ForEach-Object {
    . "$($_.FullName)"
    
}

Describe "Fmg-DocFxDocs" {

    It "Should Generate Markdown Docs" {
        $docs = "$PSScriptRoot/Docs/PsReadline"
        if((Test-Path $docs)) {
            Remove-Item $docs -Force -Recurse
        }
        Import-Module "PsReadline" -Force
        Write-ModuleHelp -Path "$PSScriptRoot/Docs/PsReadline" -Module "PsReadline"
        Test-Path "$PSScriptRoot/Docs/PsReadline" | Should Be $True 
    }
}

