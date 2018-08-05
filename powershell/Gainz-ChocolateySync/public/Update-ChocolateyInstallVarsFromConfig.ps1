function Update-ChocolateyInstallVarsFromConfig() {
    Param(
        [PSCustomObject] $Config,

        [String] $ConfigUri
    )
    <#
        {
            install: {
                path: ""
                toolsLocation: ""
                downloadUrl: ""
                version: ""
                useWindowsCompression: 
                proxy: {
                    ignore: false
                    location: 
                    user:
                    password:
                }
            }
        }
    #>
    
    if(!$Config) {
        if(!$ConfigUri) {
            $ConfigUri = $Env:ChocolateyInstallConfig

            if(!$ConfigUri) {
                $ConfigUri = $Env:CHOCOLATEY_INSTALL_CONFIG
            } 
        }
            
        if(!$ConfigUri) {
            Write-Debug "ConfigUri is not set."
            return;
        }
       
        $tmpDir = Get-ChocolateyTempInstallDirectory

        $test = [Uri]$ConfigUri
        $configPath = $null;
        if(!$test.IsFile) {
            $configPath = Join-Path $tmpDir "config.json"
            Get-WebRequestContentAsFile -Uri $ConfigUri $configPath   
            $ConfigUri = $configPath    
        } 
    
        try {
            $config  = Get-Content $ConfigUri -Raw | ConvertFrom-Json 
        } catch {
            Write-Warning "Could not load $ConfigUri" 
            Write-Debug $_.Exception
            return;
        }
    }
   

    $install = $config.install

    if($install -is [string]) {
        if($install -match "chocolatey") {
            
            if($Env:ChocolateyInstall -ne $install.path) {
                $Env:ChocolateyInstall = $install;
                if(Test-IsAdmin) {
                    [System.Environment]::SetEnvironmentVariable("ChocolateyInstall", $install.Path, "Machine")
                } else {
                    [System.Environment]::SetEnvironmentVariable("ChocolateyInstall", $install.Path, "User")
                }
            }
            return;
        }

        $Env:ChocolateyToolsLocation = $install;
        $p = Join-Path $install "chocolatey"
        if($Env:ChocolateyInstall -ne $p) {
            $Env:ChocolateyInstall = $p;
            if(Test-IsAdmin) {
                [System.Environment]::SetEnvironmentVariable("ChocolateyInstall", $install.Path, "Machine")
            } else {
                [System.Environment]::SetEnvironmentVariable("ChocolateyInstall", $install.Path, "User")
            }
        }
        return;
    }

    if($install.path) {
        if($Env:ChocolateyInstall -ne $install.path) {
            if(Test-IsAdmin) {
                [System.Environment]::SetEnvironmentVariable("ChocolateyInstall", $install.Path, "Machine")
            } else {
                [System.Environment]::SetEnvironmentVariable("ChocolateyInstall", $install.Path, "User")
            }
        }
        $Env:ChocolateyInstall = $install.path         
    } 
    if($install.toolsLocation) {
        $Env:ChocolateyToolsLocation = $install.toolsLocation
    }
    if($install.version) {
        $Env:ChocolateyVersion = $install.version 
    }
    if($install.downloadUrl) {
        $Env:ChocolatelyDownloadUrl = $install.downloadUrl;
    }
    if($install.useWindowsCompression) {
        $Env:ChocolateyUseWindowsCompression = 'true';
    }
    if($install.proxy) {
        $p = $install.proxy;
        if($p.ignore) {
            $env:ChocolateyIgnoreProxy = 'true'
        } else {
            if($p.location) { $Env:ChocolateyProxyLocation = $p.location; }
            if($p.user) { $Env:ChocolateyProxyUser = $p.user }
            if($p.password) { $Env:ChocolateyProxyPassword = $p.password; }
        }
    }   
}