function Sync-Chocolatey () {
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $true)]
        [String] $Uri,

        [Pscredential] $Credential
    )

    if($Uri -and $Uri.StartsWith(".")) {
        $Uri = (Resolve-Path $URi).Path
    }

    Write-Banner 
    $Config = Read-ChocolateyUpdateConfig -Uri $Uri
    if($Config) {
        $cfg = $Config;
        if($config.Chocolatey) { 
            $cfg = $Config.Chocolatey
            Install-Chocolatey -Config $cfg 
            Update-ChocolateySources -Config $cfg 
            Update-ChocolateyPackages -Config $cfg
        } 
       
        if($Config.boxstarter -and $Config.boxstarter.packages) {
            Install-BoxStarter
            Import-Module "$Env:ProgramData\Boxstarter\Boxstarter.Chocolatey"
            if($Config.boxstarter.feeds -and $Config.boxstarter.feeds -is [Array]) {
                $feeds = [String]::Join(";", $Config.boxstarter.feeds)
                Set-BoxstarterConfig -NugetSources $feeds
            }
            $package = $Config.bostarter.packages 
            if($package -is [Array]) {
                $package = $package[0];
            }
            #TODO: support multiple packages
            if($Credentials) {
                 Install-BoxStarterPackage -PackageName $package -Credential $Credential
            } else {
                 Install-BoxStarterPackage -PackageName $package 
            }
        }
    }
}