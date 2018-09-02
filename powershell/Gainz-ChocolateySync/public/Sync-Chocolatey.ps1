function Sync-Chocolatey () {
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $true)]
        [String] $Uri
    )

    Write-Banner 
    $Config = Read-ChocolateyUpdateConfig -Uri $Uri
    if($Config) {
        Install-Chocolatey -Config $Config
        Update-ChocolateySources -Config $Config 
        Update-ChocolateyPackages -Config $Config
    }
}