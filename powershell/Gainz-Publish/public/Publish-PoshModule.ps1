function Publish-PoshModule() {
    Param(
        [String] $Path,

        [String] $StagingDirectory,

        [String] $Respository,
        
        [String] $NugetApiKey,

        [pscredential] $PsCredential,

        [Switch] $WhatIf,

        [string[]] $Tags = $null,

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
    $files = @("$Name.psm1", "$Name.psd1", "LICENSE", "LICENSE.md", "LICENSE.txt", "README.md", "README.txt")

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
       
       "NugetApiKey" = $NugetApiKey
       "Path" = $StagingDirectory
       "WhatIf" = $WhatIf
 
       
       "Credential" = $PsCredential

    }

    if(![string]::IsNullOrWhiteSpace($Respository)) {
        $args.Add("Repository", $Respository)
    }

    if($null -ne $Tags -and $Tags.Length) {
        $args.Add("Tags", $Tags)
    }

    if($RequiredVersion) {
        $args.Add("RequiredVersion", $RequiredVersion)
    }

    if($FormatVersion) {
        $args.Add("FormatVersion", $FormatVersion)
    }
    

    if(![string]::IsNullOrWhiteSpace($ReleaseNotes)) {
        $args.Add("ReleaseNotes", $ReleaseNotes)
    }

    if(![string]::IsNullOrWhiteSpace($IconUri)) {
        $args.Add("IconUri", $IconUri)
    }

    if(![string]::IsNullOrWhiteSpace($ProjectUri)) {
        $args.Add("ProjectUri", $ProjectUri)
    }

    if(![string]::IsNullOrWhiteSpace($LicenseUri)) {
        $args.Add("LicenseUri", $LicenseUri)
    }

    return Publish-Module @args
}