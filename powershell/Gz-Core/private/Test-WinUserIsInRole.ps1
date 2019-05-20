

Function Test-WinUserIsInRole() {
    [CmdletBinding()]
    param(
        [System.Security.Principal.WindowsIdentity] $Identity,

        [String] $Role, 

        [ValidateSet("Administrator", "User", "Guest", "PowerUser", "AccountOperator", "SystemOperator", "PrintOperator", "BackupOperator", "Replicator")]
        [String] $BuiltInRole
    )

    PROCESS {
        if(!$Identity)
        {
           $Identity = [System.Security.Principal.WindowsIdentity]::GetCurrent()
        }
    
        $p = [System.Security.Principal.WindowsPrincipal]$Identity
    
        if($BuiltInRole) {
            $r = [System.Security.Principal.WindowsBuiltInRole]$BuiltInRole;
          
            return $p.IsInRole($r)
        }
    
        return $p.IsInRole($Role);
    }
}