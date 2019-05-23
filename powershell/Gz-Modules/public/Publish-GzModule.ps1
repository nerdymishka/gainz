function Publish-GzModule() {
    Param(
        [Parameter(Position = 0, Mandatory = $true)]
        [String] $Path,

        [Parameter(Position = 1, Mandatory = $true)]
        [String] $NugetApiKey,

        [String] $StagingDirectory,

        [String] $Respository,

        [String[]] $Include,

        [pscredential] $Credential,

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
        $tmp = $Env:Tmp
        if($Env:Temp) {  $tmp = $Env:Temp }
        if(!$tmp) { $tmp = $loc }

        $StagingDirectory = "$tmp/Gz-Packages"
        Write-Debug "Staging Directory: $StagingDirectory"
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

    New-Item "$StagingDirectory" -ItemType Directory

    $dirs = @("public", "private", "lib", "bin");
    $files = @("$Name.psm1", "$Name.psd1", "*.ps1", "LICENSE", "LICENSE.md", "LICENSE.txt", "README.md", "README.txt")

    if($Include -and $Include.Length) {
        foreach($f in $Include) {
            $files += $f 
        }
    }
  

    foreach($dir in $dirs)
    {
        $t = Get-Item "$Path/$dir" -ErrorAction SilentlyContinue
        if($t) {
            Write-Host $t.Name;
            Copy-Item -Path $t.FullName -Destination "$StagingDirectory/$dir" -Recurse 
        }
    }

    foreach($file in $files)
    {
        $t = Get-Item "$Path/$file" -ErrorAction SilentlyContinue
        if($t -is [Array])
        {
            Copy-Item $t -Destination "$StagingDirectory"
            continue;
        }
        if($t) {
           
            Copy-Item -Path $t.FullName -Destination "$StagingDirectory/$file"  
        }
    }
   
    $args = @{
       "NugetApiKey" = $NugetApiKey
       "Path" = $StagingDirectory
       "WhatIf" = $WhatIf.ToBool()
       "Credential" = $Credential
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
    } else {
        $files = @("$Path/RELEASENOTES.md", "$PATH/RELEASENOTES.txt")
        foreach($f in $files) {
            if(Test-Path $f) {
                $args.Add("ReleaseNotes", (Get-Content $f -Raw))
                break;
            }
        }
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