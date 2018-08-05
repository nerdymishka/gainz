$chocolatelyTmpDir = $null;

function Get-ChocolateyTempDirectory() {
    if($chocolatelyTmpDir) {
        return $chocolatelyTmpDir;
    }

    $tmpDir = $Env:Temp

    if(!$tmpDir) {
        $tmpDir = $Env:Tmp 
    
        if(!$tmpDir) {
            $tmpDir = Join-Path $env:SystemDrive "var" "tmp"
        }
    }

    $chocolatelyTmpDir = Join-Path $tmpDir "chocolatey"
  
    if(-not (Test-Path $chocolatelyTmpDir)) {
        New-Item -Path $chocolatelyTmpDir -ItemType Directory -Force | Write-Debug
    }

    return $chocolatelyTmpDir
}