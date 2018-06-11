
Param(
    [String] $PackageName = $null
)


function Build-Package() {
    Param(
        [String] $PackageName
    )

    $targetDir = (Resolve-Path "$PsScriptRoot/../src")
    $packageDir = Get-Item "$targetDir/$PackageName" -EA SilentlyContinue 
    if(!$packageDir) 
    {
        Write-Warning "Could not locate $targetDir/$PackageName"
        return;
    }

    $stepUpdate = Get-Item "$packageDir/Step-Update.ps1" -EA SilentlyContinue
    if(!$stepUpdate)
    {
        Write-Warning "Setup-Update.ps1 is missing for $PackageName"
        return;
    }

    & $stepUpdate.FullName

    Import-Module "$targetDir/Modules/Kweh-Packager"

    New-KwehChocolateyPackage "$packageDir"  -Destination "$artifacts"



    $nuspec = Get-Item "$packageDir/*.nuspec" -EA SilentlyContinue

    if(!$nuspec)
    {
        Write-Warning "$packageDir/*.nuspec is missing for $packageName"
        return;
    }

    $artifactsDir = $Global:Artifacts
    if(!$artifactsDir) 
    {
        $artifactsDir = $Env:FMG_ARTIFACTS_DIR
    }
    if(!$artifactsDir)
    {
        $artifactsDir = "$PsScriptRoot/artifacts"
    }
    $artifactsDir += "/packages"

    if(!(Test-Path $artifactsDir)) {
        New-Item $artifactsDir -ItemType Directory
    }

    choco pack $nuspec.FullName --out $artifactsDir
}

if(![string]::IsNullOrWhitespace($PackageName)) {
    Build-Package -PackageName $PackageName
}