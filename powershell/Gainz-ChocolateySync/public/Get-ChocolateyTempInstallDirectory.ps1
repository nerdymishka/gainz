$chocolateyTmpInstallDir = $null;

function Get-ChocolateyTempInstallDirectory() {

    if($chocolateyTmpInstallDir) {
        return $chocolateyTmpInstallDir;
    }
    $chocolatelyTmpDir = Get-ChocolateyTempDirectory;
    $chocolateyTmpInstallDir = Join-Path $chocolatelyTmpDir "chocoInstall"

    if(-not (Test-Path $chocolateyTmpInstallDir)) {
        New-Item $chocolateyTmpInstallDir -ItemType Directory | Write-Debug
    }
    
    return $chocolateyTmpInstallDir
}