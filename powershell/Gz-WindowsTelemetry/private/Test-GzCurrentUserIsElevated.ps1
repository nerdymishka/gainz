


if($null -eq (Get-Command Test-GzCurrentUserIsElevated -EA SilentlyContinue))
{
    $gzCurrentUserState = @{}


    function Test-GzCurrentUserIsElevated() {
        [CmdletBinding()]
        Param(
            
        )
    
        Process {
            switch([Environment]::OsVersion.Platform) {
                "Win32NT" {
                    if($gzCurrentUserState.ContainsKey($Env:USERNAME)) {
                        return $gzCurrentUserState[$Env:USERNAME];
                    }


                    $identity = [System.Security.Principal.WindowsIdentity]::GetCurrent()
                    
                    
                    $admin = [System.Security.Principal.WindowsBuiltInRole]::Administrator
                    $gzCurrentUserState[$Env:USERNAME] = ([System.Security.Principal.WindowsPrincipal]$identity).IsInRole($admin)

                    return $gzCurrentUserState[$Env:USERNAME];
                }
                "Unix" {
                    $content = id -u
                    if($content -eq "0") {
                        return $true;
                    } 
        
                    return $false;
                }
                Default {
                    $plat = [Environment]::OsVersion.Platform
                    Write-Warning "$plat Not Supported"
                    return $false
                }
            }
        }
    }
}


