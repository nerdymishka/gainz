<#
    Unsafe reason: These tests pose risk if they go awry as the tests will download
    chocolatey which does modify environment variables. 

    Run unsafe tests on a VM.
#>

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

    It "Should update sources" {
        $cfg = Read-ChocolateyUpdateConfig "$PSScriptRoot/config.json"
        Update-ChocolateySources -Config $cfg
        $result = $false;

        $sources = choco source
        foreach($line in $sources) {
            if($line -match "nerdymishka") {
                $result = $true;
                break;
            }
        }

        $result | Should Be $true 

        $cfg = Read-ChocolateyUpdateConfig "$PSScriptRoot/config-rm.sample.json"
        Update-ChocolateySources -Config $cfg
        $result = $false;
        $sources = choco source
        foreach($line in $sources) {
            if($line -match "nerdymishka") {
                $result = $true;
                break;
            }
        }
        $result | Should Be $false;
    }

    It "Should install chocolatey" {
        $opt = Get-Item "$PSScriptRoot/opt" -EA SilentlyContinue
        if($opt) {
            $opt | Remove-Item -Recurse -Force
            $opt.Exists | Should Be $false 
        }

      
        $cfg = Read-ChocolateyUpdateConfig "$PSScriptRoot/config.json"
        Install-Chocolatey -Config $cfg -Force

        $opt = Get-ITem "$PSScriptRoot/opt" -EA SilentlyContinue
        $opt.Exists | Should Be $true
    }

    It "Should install packages" {
        $putty = Get-Item "$PSScriptRoot/opt/chocolatey/lib/putty" -EA SilentlyContinue
        if($putty) {
            $putty | Remove-Item -Recurse -Force 
            $putty.Refresh()
            $putty.Exists | Should Be $false 
        }
       
    

        $cfg = Read-ChocolateyUpdateConfig "$PSScriptRoot/config.json"
        Update-ChocolateyPackages -Config $cfg

        $putty = Get-Item "$PSScriptRoot/opt/chocolatey/lib/putty" -EA SilentlyContinue
        $putty.Refresh()
        $putty.Exists | Should Be $true 
    }

    It "Should remove packages" {
        $cfg = Read-ChocolateyUpdateConfig "$PSScriptRoot/config-rm.sample.json"
        Update-ChocolateyPackages -Config $cfg

        # the folder removal is slow
        sleep 3
        
        $putty = Get-Item "$PSScriptRoot/opt/chocolatey/lib/putty" -EA SilentlyContinue
        $putty | Should Be $null;

    }


    # Clean up
    $opt = Get-Item "$PSScriptRoot/opt" -EA SilentlyContinue
    if($opt) {
       $opt | Remove-Item -Force -Recurse
    }

  
    if(Test-IsAdmin) {
        [Environment]::SetEnvironmentVariable("ChocolatelyInstall", $originalInstall, "Machine")
        [Environment]::SetEnvironmentVariable("ChocolatelyToolsLocation", $originalTools, "Machine")
    } else {
        [Environment]::SetEnvironmentVariable("ChocolatelyInstall", $originalInstall, "User")
        [Environment]::SetEnvironmentVariable("ChocolatelyToolsLocation", $originalTools, "User")
    }
}