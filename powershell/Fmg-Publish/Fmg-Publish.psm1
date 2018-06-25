

function Publish-FmgModule() {
    Param(
        [String] $Path,

        [String] $StagingDirectory,

        [String] $Respository,
        
        [String] $NugetApiKey,

        [pscredential] $PsCredential,

        [Switch] $WhatIf,

        [string[]] $Tags,

        [String] $ReleaseNotes,

        [String] $LicenseUri,

        [String] $IconUri,

        [String] $ProjectUri,

        [version] $RequiredVersion,

        [version] $FormatVersion,

        [switch] $Force 
    )

    if([String]::IsNullOrWhiteSpace($StagingDirectory)) {
        $loc = (Get-Location).Path
        $StagingDirectory = "$loc/Packages"
    }

    $Path = (Resolve-Path $Path).Path
    if(-not (Test-Path $Path)) {
        Write-Warning "Could not locate $Path";
        return;
    }

    $name = Split-Path $Path -Leaf;
    $StagingDirectory = "$StagingDirectory/$Name"
   

    if(Test-Path "$StagingDirectory") {
        Remove-Item "$StagingDirectory" -Force -Recurse
    }

    Write-Host $StagingDirectory
    New-Item "$StagingDirectory" -ItemType Directory

    $dirs = @("public", "private", "lib", "bin");
    $files = @("$Name.psm1", "$Name.psd1", "LICENSE", "README.md")

    foreach($dir in $dirs)
    {
        $t = Get-Item "$Path/$dir" -ErrorAction SilentlyContinue
        if($t) {
            Copy-Item -Path $t -Destination "$StagingDirectory/$dir" -Recurse 
        }
    }

    foreach($file in $files)
    {
        $t = Get-Item "$Path/$file" -ErrorAction SilentlyContinue
        if($t) {
            Copy-Item -Path $t -Destination "$StagingDirectory/$file"  
        }
    }
   
    $args = @{
       "Repository" = $Respository
       "NugetApiKey" = $NugetApiKey
       "Path" = $StagingDirectory
       "WhatIf" = $WhatIf
       "RequiredVersion" = $RequiredVersion
       "FormatVersion" = $FormatVersion 
       "Tags" = $Tags 
       "Credential" = $PsCredential
       "ReleaseNotes" = $ReleaseNotes
       "LicenseUri" = $LicenseUri
       "IconUri" = $IconUri
       "ProjectUri" = $ProjectUri 
    }

    return Publish-Module @args
}