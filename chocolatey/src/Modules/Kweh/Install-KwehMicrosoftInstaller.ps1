
function Install-KwehMicrosoftInstaller() {
    Param(
        [String] $Path, 
        [String] $Log,
        [String[]] $ArgumentList,
        [Switch] $SkipAdminCheck
    )


    

    $requireAdmin = $false;
    $isElevated = $false;
    if(!$SkipAdminCheck) {
        $requireAdmin = $true;
        if($ArgumentList -eq $null -or $ArgumentList.Count -eq 0) {
            $isElevated = Test-IsElevated 
        } else {
            foreach($arg in $ArgumentList) {
                if($arg -match "MSIINSTALLPERUSER=1") {
                    $requireAdmin = $false 
                }
            }
        }
    }



    if($requireAdmin -and !$isElevated) {
        $args = [string]::Join(" ", $ArgumentList)
        $next = "-Path `"$Path`" -Log `"$Log`""
        if($ArgumentList -ne $null -and $ArgumentList.Length -gt 0) {
            $next += " -ArgumentList `"$args`""
        }

        $modules = "$PSScriptPath/Kweh.psm1"

        # This should only be set inside of chocolatey.
        if($env:ChocolateyPackageName) {
            $modules = "$Env:ChocolateyInstall/helpers/chocolateyInstaller.psm1"
        }
        
        Invoke-ElevatedPowershell -Command "Install-Msi" -Arguments $next -Module $modules 
    }
   

    $Path = (Resolve-Path $Path).Path

    msiexec.exe /i $PATH /qn /norestart @ArgumentList 
    Start-Sleep 1
    return $LASTEXITCODE 
}
Set-Alias -Name Install-MicrosoftInstaller -Value Install-KwehMicrosoftInstaller  
