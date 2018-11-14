


function Update-PoshPackages() {
    Param(
        [PsCustomObject] $Config
    )

    if($Config.packages)
    {
        $installed = Get-Module -ListAvailable

        $packages = $config.packages
        if($packages -is [Array])
        {
            foreach($package in $packages) {
                $installedMod = $null;
                $found = $false;
                $sessionMod = $null;

                if($package -is [String])
                {
                    $parts = $package.Trim().Split(' ')
                    $name = $parts[0];
                    $found = $false;

                    $required = $false;
                    $minimum = $false;

                    foreach($part in $parts) {
                        if($part -eq "-RequiredVersion") {
                            $required = $true;
                            continue;
                        }

                        if($part -eq "-MiniumuVersion") {
                            $required = $true;
                            continue;
                        }
                        if($required -is [Boolean] -and $required) {
                            $required = $part;
                            continue;
                        }

                        if($minimum -is [Boolean] -and $minimum) {
                            $minimum = $part;
                            continue;
                        }
                    }

                    if($PSVersionTable.PSVersion -is [Version]) {
                        if($required) {
                            $required = New-Object System.Version -ArgumentList $required
                        }
                        if($minimum) {
                            $minimum = New-Object System.Version -ArgumentList $minimum
                        }
                    } else {
                        if($required) {
                            $required = New-Object System.Management.Automation.SemanticVersion -ArgumentList $required
                        }
                        if($minimum) {
                            $minimum = New-Object System.Management.Automation.SemanticVersion -ArgumentList $minimum
                        }
                    }

                   

                    foreach($mod in $installed)
                    {
                        if($mod.Name -eq $name) {
                            $found = $true;
                            $installedMod = $mod;
                            break;
                        }
                    }
                } elseif($package -is [PsCustomObject]) {

                    $name = $package.Name
                    $required = $package.RequiredVersion
                    $minimum = $package.MiniumuVersion
                    if($PSVersionTable.PSVersion -is [Version]) {
                        if($required) {
                            $required = New-Object System.Version -ArgumentList $required
                        }
                        if($minimum) {
                            $minimum = New-Object System.Version -ArgumentList $minimum
                        }
                    } else {
                        if($required) {
                            $required = New-Object System.Management.Automation.SemanticVersion -ArgumentList $required
                        }
                        if($minimum) {
                            $minimum = New-Object System.Management.Automation.SemanticVersion -ArgumentList $minimum
                        }
                    }

                    $splat = @{};
                    $package | Get-Member -MemberType NoteProperty | ForEach-Object {
                        $key = $_.Name 
                        $Value = $_.Value; 

                        if($key -eq "name") {
                            return;
                        }

                        $splat.Add($key, $value)
                    }
                }


                if($found)
                {   
                    if(($required -and $required -gt $installed.Version) -or 
                        ($minimum -and $minimum -gt $installed.Version)) {

                        $installedMod | Uninstall-Module -Force 

                        $sessionMod = Get-Module $name -Ea SilentlyContinue;

                        if($sessionMod) {
                            Remove-Module $name -Force;
                        }

                        if($package -is [String]) {
                            Install-Module $package 
                        } else {
                            Install-Module $name @splat 
                        }
                        
                        Import-Module $name 
                    }

                    continue;
                }

                if($package -is [String]) {
                    Install-Module $package 
                } else {
                    Install-Module $name @splat 
                }
                
                Import-Module $name 
            }
        }
    }
}