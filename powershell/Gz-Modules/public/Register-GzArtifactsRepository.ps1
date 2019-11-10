

$gzPublishArtifactsDirectory = $null

function Set-GzPublishArtifactsDirectory() {
    Param(
        [Parameter(Position = 0)]
        [String] $Path 
    )

    $gzPublishArtifactsDirectory = $Path;
}

function Get-GzPublishArtifactsDirectory() {

    if(!$gzPublishArtifactsDirectory) {
        $gzPublishArtifactsDirectory = "$HOME/.gz/artifacts"
    }
}


function Register-GzArtifactsRepository() {
    Param(
        [Parameter(Position = 0)]
        [String] $Name = "GzArtifacts"
    )
    $artifactsDir = Get-GzPublishArtifactsDirectory
    $feed = "$artifactsDir/feed"
    $repo = Get-PSRepository -Name $Name -EA SilentlyContinue
    if(!(Test-Path $feed))
    {
        New-Item -ItemType Directory $feed 
    }

    if($repo) {
        if($repo.PublishLocation -eq $feed) {
            return 
        }
        Unregister-PSRepository -Name $name 
    }

    Register-PSRepository -Name $name `
        -ScriptPublishLocation $feed `
        -SourceLocation = $feed `
        -PublishLocation = $feed `
        -ScriptSourceLocation = $feed `
        -InstallationPolicy Trusted 
}