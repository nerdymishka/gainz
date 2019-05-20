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
            {$PSItem.StartsWith("Win")} {
                $gzCurrentUserIsAdministrator = Test-WinUserIsInRole -BuiltInRole "Administrator"
            }
            {$PSItem -eq "MacOSX" -or $PSItem -eq "Unix"} {
                $gzCurrentUserIsAdministrator = Test-UnixUserIsInRole -Group "Root"
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