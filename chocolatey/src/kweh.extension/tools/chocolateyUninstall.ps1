$mod = Get-Module Kweh -ErrorAction SilentlyContinue
if($mod) {
    Remove-Module $mod -Force 
}

if(Test-Path "$env:ChocolateyInstall\extensions\kweh") {
    $scripts = gci "$env:ChocolateyInstall\extensions\kweh"
    foreach($s in $scripts){
        Remove-Item $s.FullName -Force
    }
    try {
        Remove-Item "$env:ChocolateyInstall\extensions\kweh" -Force 
    } catch {
        Start-Job -ScriptBlock {
            Start-Sleep 5
            try {
                Remove-Item "$env:ChocolateyInstall\extensions\kweh" -Force 
            } catch {

            }
        }
    }
    
}