function Sync-Chocolatey () {
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $true)]
        [String] $Uri,

        [Pscredential] $Credentials
    )

    if($Uri -and $Uri.StartsWith(".")) {
        $Uri = (Resolve-Path $URi).Path
    }

    Write-Banner 
    $Config = Read-ChocolateyUpdateConfig -Uri $Uri
    if($Config) {
        Install-Chocolatey -Config $Config
        Update-ChocolateySources -Config $Config 
        Update-ChocolateyPackages -Config $Config
        if($Config.boxstarter -and $Config.boxstarter.packages) {
            Install-BoxStarter
            Import-Module "$Env:ProgramData\Boxstarter\Boxstarter.Chocolatey"
            if($Config.boxstarter.feeds -and $Config.boxstarter.feeds -is [Array]) {
                $feeds = [String]::Join($Config.boxstarter.feeds)
                Set-BoxstarterConfig -NugetSources $feeds
            }
            $package = $Config.bostarter.packages 
            if($package -is [Array]) {
                $package = $package[0];
            }
            #TODO: support multiple packages
            if($Credentials) {
                 Install-BoxStarterPackage -PackageName $package -Credential $Credentials
            } else {
                 Install-BoxStarterPackage -PackageName $package 
            }
        }
    }
}