$gzCurrentUserIsAdministrator = $null

function Test-UserIsAdministrator() {
    
    [CmdletBinding()]
    Param(
        [Switch] $Force 
    )
    
    PROCESS {
        if($null -ne $gzCurrentUserIsAdministrator -and !($Force.ToBool())) {
            return $gzCurrentUserIsAdministrator
        }

        $platform = Get-OsPlatform 

        switch($platform) {
            "Win32NT|Win32Windows|Win32s" {
                $identity = [System.Security.Principal.WindowsIdentity]::GetCurrent()
                $admin = [System.Security.Principal.WindowsBuiltInRole]::Administrator
                $gzCurrentUserIsAdministrator = ([System.Security.Principal.WindowsPrincipal]$identity).IsInRole($admin)
            }
            "Unix|MacOSX" {
                $content = id -u
                if($content -eq "0") {
                    $gzCurrentUserIsAdministrator = $true;
                } 
    
                $gzCurrentUserIsAdministrator = $false;
            }
            Default {
                $plat = [Environment]::OsVersion.Platform
                Write-Warning "$plat Not Supported"
                $gzCurrentUserIsAdministrator = $false
            }
        }

        return $gzCurrentUserIsAdministrator
    }
}