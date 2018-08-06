
function Install-Chocolatey() {
    Param(
        [Parameter(Position = 0, ValueFromPipeline)]
        [PsCustomObject] $Config,

        [Parameter(ValueFromPipeline = $true)]
        [String] $ConfigUri,

        [Switch] $Force 
    )

    Write-Banner;
    
    $choco = Get-Command choco -ErrorAction SilentlyContinue
    $location = Get-ChocolateyInstallLocation 

    if($choco -or (Test-Path $location)) {
        if(!$Force.ToBool()) {
            Write-Debug "Chocolatey is already installed. Use -Force to re-install."
            return;
        }
    }

    function Update-ProcessPath() {

        $chocolateyInstall = Get-ChocolateyInstallLocation
        $bin = Join-Path $chocolateyInstall 'bin'

        if(! ($env:Path -match ($bin.Replace("\", "\\")))) {
            $Env:Path = [System.Environment]::GetEnvironmentVariable("Path", "Machine")
        }
    }

    function Copy-ChocolateyPackage() {
        Param(
            [Parameter(Position = 1)]
            [String] $Path 
        )

        $chocolateyInstall = Get-ChocolateyInstallLocation
        $chocolateyPkgDir = Join-Path $chocolateyInstall 'lib'
        $chocolateyPkgDir = Join-Path $chocolateyPkgDir "chocolatey"
        $chocolateyNupkg = Join-Path $chocolateyPkgDir 'chocolatey.nupkg'

        if(-not (Test-Path $chocolateyPkgDir)) {
            New-Item $chocolateyPkgDir -ItemType Directory -Force | Out-Null 
        }

        Write-Debug 'Ensuring chocolatey.nupkg is in the lib folder'
        Copy-Item $Path $chocolateyNupkg -Force  
    }

    Update-PowerShellOutputRedirection
    Update-SecurityProtocol
    Update-ChocolateyInstallVarsFromConfig -Config $Config -ConfigUri $ConfigUri


    $uri = Get-ChocolateyDownloadUri
    $tmpDir = Get-ChocolateyTempInstallDirectory
    $zip = Join-Path $tmpDir "chocolatey.zip"



    Write-Host "Getting Chocolatey from $uri." -ForegroundColor Green
    Write-Host "Downloading $uri to $zip" -ForegroundColor Green
    Save-WebRequestContentAsFile -Uri $uri -Destination $zip  
    Write-Host "Expanding $zip in $tmpDir"
    Expand-ChocolateyArchive -Path $zip -Destination $tmpDir -Force

    Write-Host "Executing Chocolatey Install"  -ForegroundColor Green
    Invoke-ChocolateySetupScript


    Write-Host "Ensure Path for current session is updated"  -ForegroundColor Green
    Update-ProcessPath

    Write-Host "Copy chocolatey.nupkg to install location"  -ForegroundColor Green
    Copy-ChocolateyPackage -Path $tmpDir
}

