
$toolsDir = Split-PAth $MyInvocation.MyCommand.Path 

$mod = Get-Module Kweh -ErrorAction SilentlyContinue
if($mod) {
    Remove-Module $mod -Force
}

if(Test-PAth "$env:ChocolateyInstall\extensions\kweh") {
    $scripts = gci "$env:ChocolateyInstall\extensions\kweh"
    foreach($s in $scripts){
        Remove-Item $s.FullName -Force
    }
}

if(! (Test-Path "$env:ChocolateyInstall\extensions\kweh")) {
    New-Item "$env:ChocolateyInstall\extensions\kweh" -ItemType Directory
}

$files = Get-ChildItem "$toolsDir\Kweh"
foreach($file in $files) {
    Copy-Item $file.FullName "$env:ChocolateyInstall\extensions\kweh" -Force -Recurse
}
