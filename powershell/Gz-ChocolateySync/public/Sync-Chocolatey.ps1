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
            if(Test-Path "$Env:ChocolateyInstall\lib\chocolatey.nupkg") {
                Remove-Item "$Env:ChocolateyInstall\lib\chocolatey.nupkg"
            }
            Update-ChocolateySources -Config $cfg 
            Update-ChocolateyPackages -Config $cfg
        } 
       
        if($Config.boxstarter -and $Config.boxstarter.packages) {
            $cfg = $Config.Chocolatey 
            if(!$cfg) { 
                $cfg = [PSCustomObject]{} 
                Install-Chocolatey $cfg 
            }
            
            Install-BoxStarter
            Import-Module "$Env:ProgramData\Boxstarter\Boxstarter.Chocolatey"
            $feeds = $Config.boxstarter.feeds;
            $packages = $Config.boxstarter.packages;
            if($feeds -and $feeds -is [Array] -and !($feeds -is [String])) {
                $feeds = [String]::Join(";", $Config.boxstarter.feeds)
                Set-BoxstarterConfig -NugetSources $feeds
            }
            
            if($packages -is [Array] -and !($packages -is [String])) {
                $packages = $packages[0];
            }
            #TODO: support multiple packages
            if($Credential) {
                 Install-BoxStarterPackage -PackageName $packages -Credential $Credential
            } else {
                 Install-BoxStarterPackage -PackageName $packages 
            }
        }
    }
}