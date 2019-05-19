
function Get-OsPlatform() {
    [CmdletBinding()]
    Param() 

    PROCESS {
        return [Environment]::OSVersion.Platform.ToString();
    }
}