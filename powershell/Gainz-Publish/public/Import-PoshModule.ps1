function Import-PoshModule() {
    Param(
        [String] $ModuleName,
        
        [Switch] $ForceInstall,

        [Switch] $Force,

        [Switch] $PassThru,

        [pscredential] $Credential,

        [Version] $MinimumVersion,

        [Version] $MaximumVersion,

        [Version] $RequiredVersion,

        [string] $Respository,
    
        [string] $Scope,

        [Switch] $Latest,

        [switch] $SkipPublisherCheck
    )

    $all = $Scope -eq "Machine" -or $Scope -eq "Global" -or $Scope -eq "AllUsers";
    $installScope = "CurrentUser";
    if($all) {
        $installScope = "AllUsers";
    }
    if([string]::IsNullOrWhiteSpace($Scope)) {
        $installScope = $null;
    }

    $installSplat = @{};
    $installSplat | Add-ObjectParameter -Name "MinimumVersion" -Value $MinimumVersion
    $installSplat | Add-ObjectParameter -Name "MaximumVersion" -Value $MaximumVersion
    $installSplat | Add-ObjectParameter -Name "RequiredVersion" -Value $RequiredVersion
    $installSplat | Add-StringParameter -Name "Scope" -Value $importScope
    $installSplat | Add-SwitchParameter -Name "Force" -Value $ForceInstall.ToBool()
    $installSplat | Add-SwitchParameter -Name "SkipPublisherCheck" -Value $SkipPublisherCheck.ToBool()
    $installSplat | Add-StringParameter -Name "Scope" -Value $installScope
    $installSplat | Add-ObjectParameter -Name "Credential" -Value $Credential


    if(!$Force.ToBool()) {
        $mod = Get-Module $ModuleName -EA SilentlyContinue

        if($mod)
        {
            $import = $false;
            if($RequiredVersion -and $RequiredVersion -ne $mod.Version) {
                Install-Module $ModuleName @installSplat -EA SilentlyContinue
                Update-Module -RequiredVersion $RequiredVersion -EA SilentlyContinue 
                $mod = Get-Module $ModuleName -EA SilentlyContinue
                if($mod.Version -ne $RequiredVersion) {
                    $import = $true;
                }   
            }

            if($MinimumVersion -and $mod.Version -lt $MinimumVersion) {
                Install-Module $ModuleName @installSplat -EA SilentlyContinue
                Update-Module $ModuleName -MaximumVersion MaximumVersion -EA SilentlyContinue
                if($mod.Version -lt $MinimumVersion) {
                    $import = $true;
                }  
            }

            if($MaximumVersion -and $mod.Version -gt $MaximumVersion) {
                Install-Module $ModuleName @installSplat -EA SilentlyContinue
                Update-Module $ModuleName -MaximumVersion $MaximumVersion -EA SilentlyContinue
                $mod = Get-Module $ModuleName -EA SilentlyContinue
                if($mod.Version -gt $MaximumVersion) {
                    $import = $true;
                }  
            }

            if(!$import) {
                if($PassThru.ToBool()) {
                    return $mod;
                }

                return;
            }
        }
    }
   

    $importScope = "Local";
    if($all) {
        $importScope = "Global"
    }
    if([string]::IsNullOrWhiteSpace($Scope)) {
        $importScope = $null;
    }

    $importSplat = @{}
    $importSplat | Add-ObjectParameter -Name "MinimumVersion" -Value $MinimumVersion
    $importSplat | Add-ObjectParameter -Name "MaximumVersion" -Value $MaximumVersion
    $importSplat | Add-ObjectParameter -Name "RequiredVersion" -Value $RequiredVersion
    $importSplat | Add-SwitchParameter -Name "PassThru" -Value $true
    $importScope | Add-StringParameter -Name "Scope" -Value $importScope
    $importScope | Add-SwitchParameter -Name "Force" -Value $Force.ToBool()

    $mod = Import-Module @importSplat

    if(!$mod) {
        Install-Module $ModuleName @installSplat
        $mod = Import-Module $ModuleName @importSplat
    }


    if($PassThru.ToBool()) {
        return $mod;
    }
}