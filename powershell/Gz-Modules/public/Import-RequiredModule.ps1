
function Import-RequiredModule() {
    Param(
        [String] $Name,

        [String] $RequiredVersion,

        [String] $MinimumVersion,

        [Switch] $AllowPrerelease,

        [Switch] $SkipPublisherCheck,

        [Switch] $NoClobber,

        [Switch] $AllowClobber,

        [Switch] $AcceptLicense,

        [ValidateSet("Global", "Local")]
        [String] $ImportScope = "Global",

        [ValidateSet("AllUsers", "CurrentUser")]
        [String] $Scope = "AllUsers",

        [pscredential] $Credential,

        [String] $Repository,

        [String] $Prefix 
    )


    $mod = Get-Module $Name -EA SilentlyContinue
    $hasRequiredVersion = ![String]::IsNullOrWhiteSpace($RequiredVersion)
    $hasMinimumVersion = ![String]::IsNullOrWhiteSpace($MinimumVersion)
    $installed = $false;
    if($mod)
    {
        if(!$hasRequiredVersion -and !$hasMinimumVersion) {
            return;
        }

        if($hasRequiredVersion) {
            $v = New-Object System.Version (Remove-VersionSuffix $RequiredVersion)
            if($mod.Version -eq $v) {
                return;
            } 
        }

        if($hasMinimumVersion) {
            $v = New-Object System.Version (Remove-VersionSuffix $MinimumVersion)
            if($mod.Version -ge $v) {
                return;
            } 
        }

        
        Remove-Module $Name -Force
        $installed = $true;
    }

    $importArgs = @{ Name = $Name; PassThru = $true; Scope = $ImportScope }

    if(![String]::IsNullOrWhiteSpace($Prefix)) {
        $importArgs.Add("Prefix", $Prefix);
    }


    $installArgs = @{ Name = $Name; Force = $true; Scope = $Scope  }
    $updateArgs = @{ Name = $Name; Force = $true; }
    if($hasRequiredVersion) {
        $importArgs.Add("RequiredVersion", $RequiredVersion) 
        $installArgs.Add("RequiredVersion", $RequiredVersion) 
        $updateArgs.Add("RequiredVersion", $RequiredVersion)
    } elseif($hasMinimumVersion) {
        $importArgs.Add("MinimumVersion", $MinimumVersion) 
        $installArgs.Add("MinimumVersion", $MinimumVersion)
    }

    

    $mod = Import-Module  @importArgs -EA SilentlyContinue
    if(!$mod) {
        if($AllowPrerelease.ToBool()) {
            $installArgs.Add("AllowPrerelease", $True);
            $updateArgs.Add("AllowPrelease", $true)
        }

        if($SkipPublisherCheck.ToBool()) {
            $installArgs.Add("SkipPublisherCheck", $SkipPublisherCheck);
        }

        if($AllowClobber.ToBool()) {
            $installArgs.Add("AllowClobber", $True)
        }

        if($AcceptLicense.ToBool()) {
            $updateArgs.Add("AcceptLicense", $True);
            $updateArgs.Add("AcceptLicense", $True);
        }

        if($Credential) {
            $installArgs.Add("Credential", $Credential)
            $updateArgs.Add("Credential", $Credential)
        }

        if(![string]::IsNullOrWhiteSpace("Repository")) {
            $installArgs.Add("Repository", $Repository)
        }

        if($installed) {
            Update-Module @updateArgs
        } else {
            Install-Module @installArgs
        }
       
        Import-Module @importArgs
    }
}