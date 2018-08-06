

if(!$PSScriptRoot) {
    $PSScriptRoot = $MyInovocation.PSScriptRoot
}

if(!$PSScriptRoot) {
    $PSScriptRoot = Split-Path $MyInovocation.MyCommand.Path
}

Get-Item "$PsScriptRoot\..\private\*.ps1" | ForEach-Object {
    . "$($_.FullName)"
}


Get-Item "$PsScriptRoot\..\public\*.ps1" | ForEach-Object {
    . "$($_.FullName)"
}


Describe "Gainz-ChocolateySync" {
    $originalInstall = $Env:ChocolatelyInstall
    $originalTools = $Env:ChocolateyToolsLocation

    $content = Get-Content "$PSScriptRoot/config.sample.json" -Raw
    $r = $PSScriptRoot.Replace("\", "/")
    $content = $content.Replace("{installPath}", "$r/opt/chocolatey")
    $content = $content.Replace("{toolsLocation}", "$r/opt/tools")
    $content | Out-File "$PSScriptRoot/config.json" -Encoding "utf8" -Force
 
    It "Should read the update config" {
        $cfg = Read-ChocolateyUpdateConfig "$PSScriptRoot/config.json"
       $cfg | Should not be $null;
       $cfg.update | Should Be $True
    }

    It "Should install chocolatey" {
        $opt = Get-ITem "$PSScriptRoot/opt" -EA SilentlyContinue
        if($opt) {
            $opt | Remove-ITem -Recurse -Force
        }

        $opt.Exists | Should Be $false 

       

        $cfg = Read-ChocolateyUpdateConfig "$PSScriptRoot/config.json"
        Install-Chocolatey -Config $cfg -Force

        $opt = Get-ITem "$PSScriptRoot/opt" -EA SilentlyContinue
        $opt.Exists | Should Be $true
    }

    if(Test-IsAdmin) {
        [Environment]::SetEnvironmentVariable("ChocolatelyInstall", $originalInstall, "Machine")
        [Environment]::SetEnvironmentVariable("ChocolatelyToolsLocation", $originalTools, "Machine")
    } else {
        [Environment]::SetEnvironmentVariable("ChocolatelyInstall", $originalInstall, "User")
        [Environment]::SetEnvironmentVariable("ChocolatelyToolsLocation", $originalTools, "User")
    }
}