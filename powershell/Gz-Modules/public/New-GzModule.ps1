function New-GzModule() {
    Param(
        [String] $Name,

        [String] $Destination
    )

    if([string]::IsNullOrWhiteSpace($Destination)) {
        $Destination = (Get-Location).Path
    }

    $directories = @(
        "$Destination/$Name",
        "$Destination/$Name/public",
        "$Destination/$Name/private",
        "$Destination/$Name/lib",
        "$Destination/$Name/bin",
        "$Destination/$Name/test"
    )

    $files = @{
        "$Destination/$Name/$Name.psm1" = @"

if(!`$PSScriptRoot) {
    `$PSScriptRoot = `$MyInovocation.PSScriptRoot
}

if(!`$PSScriptRoot) {
    `$PSScriptRoot = Split-Path `$MyInovocation.MyCommand.Path
}

Get-Item "`$PsScriptRoot\private\*.ps1" | ForEach-Object {
    . "`$(`$_.FullName)"
}

Get-Item "`$PsScriptRoot\public\*.ps1" | ForEach-Object {
    . "`$(`$_.FullName)"
}


Export-ModuleMember -Function  @(

)   
"@
        "$Destination/$Name/test/Unit.Tests.ps1" = @"
Import-Module "`$PsScriptRoot/../*.psm1" -Force

InModuleScope "$Name" {

    Describe "My-Function" {

        It "Should do x" {

        }
    }
}
"@
        "$Destination/$Name/README.md" = @"
# $Name

"@
        "$Destination/$Name/LICENSE.md" = ""

    }
        
    foreach($directory in $directories) {
        if(!(Test-Path $directory)) {
            New-Item $directory -ItemType Directory | Write-Debug
        }
    }

    foreach($f in $files.Keys) {
        if(!(Test-Path $f)) {
            Set-Content -Path $f -Value $files[$f]
        }
    }

    return [PSCustomObject]@{
        Name = $Name 
        Destination = $Destination
        Directories = $directories
        Files = $files
    }
}