
$projectLocation = "$PSScriptRoot"

$lastPath = $null;
$config = $null;

function Read-GainzBuildConfig() {
    Param(
        [Parameter(Position = 0)]
        [String] $Path 
    )

    if([string]::IsNullOrWhiteSpace($Path)) {
        $Path = $pwd.Path
        $Path += "./.gainz.json"
    }

    if($config)
    {
        if($Path -eq $lastPath) {
            return $config;
        }

        $config = $null;
    }

    if(!(Test-Path $Path)) {
        Write-Warning "A config could not be found at: $Path";
        return $null;
    }

    $config = Get-Content $Path -Raw | ConvertFrom-Json
    $lastPath = $Path; 
    $config | Add-Member -MemberType NoteProperty -Name "location" -Value $Path -Force
    return $config; 
}

function Start-GainzBuild() {
    Param(
        [ScriptBlock] $Block 
    )

    $config = Read-GainzBuildConfig
    if(!$config) {
        Write-Error "config is missing";
        exit;
    }

    $relativePath = $config.paths.tmp;
    if(!$relativePath) {
        $root = $config.paths.root 
        if(!$root) {
            $root = $config.location
        }

        $relativePath = "$root/.tmp";
    }
    $buildFile += "$relativePath/.tmp/build.json";

    $json = $null
    if(!(Test-PAth $buildFile))
    {
        $json = [PSCustomObject]@{
            datetime = (Get-Date).ToUniversalTime()
            revision = 1
            id = 1
            lastReset = (Get-Date).ToUniversalTime()
        }
    } else {
        $json = Get-Content $buildFile -Raw | ConvertFrom-Json
    }
    
    $now = (Get-Date).ToUniversalTime()
    if($now.Day -gt $json.lastReset.Day) {
        $json.lastReset = $now;
        $json.revision = 1;
    }
    $json.id = $json.id + 1;
    $json.datetme = (Get-Date).ToUniversalTime();

    $json | ConvertTo-Json  | Out-File $buildFile -Encoding "UTF-8";
}


function Get-GainzArtifactsPath() {
    
    $config = Read-GainzBuildConfig
    if(!$config) {
        Write-Error "config is missing";
        exit;
    }

    $relativePath = $config.paths.artifacts 
    if(!$relativePath) {
        $root = $config.paths.root 
        if(!$root) {
            $root = $config.location
        }

        $relativePath = "$root/.tmp/artifacts";
    }

    if(!(Test-Path $relativePath)) {
        New-Item -PAth $relativePath -ItemType Directory
    }
}

function Clear-GainzArtifactsPath() {

    $config = Read-GainzBuildConfig
    if(!$config) {
        Write-Error "config is missing";
        exit;
    }

    $relativePath = $config.paths.artifacts 
    if(!$relativePath) {
        $root = $config.paths.root 
        if(!$root) {
            $root = $config.location
        }

        $relativePath = "$root/.tmp/artifacts";
    }

    if((Test-Path $relativePath)) {
        Remove-Item $relativePath -Force -Recurse
    }
}