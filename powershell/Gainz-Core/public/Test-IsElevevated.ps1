function Test-IsElevated() {
    [CmdletBinding]
    Param(
        
    )

    Process {
        switch([Environment]::OsVersion.Platform) {
            "Win32NT" {
                $identity = [System.Security.Principal.WindowsIdentity]::GetCurrent()
                $admin = [System.Security.Principal.WindowsBuiltInRole]::Administrator
                return ([System.Security.Principal.WindowsPrincipal]$identity).IsInRole($admin)
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