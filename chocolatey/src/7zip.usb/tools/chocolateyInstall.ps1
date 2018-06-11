$toolsDir = $PSScriptRoot

if(!$toolsDir -or !$toolsDir.EndsWith("tools")) {   
    $toolsDir = Split-Path $MyInvocation.MyCommand.Path
}

Install-KwehPackage -Path "$toolsDir/kweh.json" 