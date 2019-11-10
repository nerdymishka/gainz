

Get-Item "$PSScriptRoot/public/*.ps1" | ForEach-Object {
    . "$($_.FullName)"
}

Export-ModuleMember -Function @(
    'Get-GzPublishArtifactsDirectory',
    'Publish-GzModule',
    'Register-GzArtifactsRepository',
    'Set-GzPublishArtifactsDirectory'
)