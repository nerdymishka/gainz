function Register-GzRegistryUserHive()
{
<#
.SYNOPSIS
    Register HKU for the users hive 
.DESCRIPTION
     Register HKU for the users hive 
.EXAMPLE
    PS C:\> Register-GzRegistryUserHive
    New-PSDrive -PSProvider Registry -Name HKU -Root Registry::HKEY_USERS 
.INPUTS
    None
.OUTPUTS
    Boolean
#>

    [CmdletBinding()]
    Param()

    PROCESS {
        $drives = Get-PsDrive 
    
        foreach($d in $drives)
        {
       
            if($d.Name -eq  "HKU") {
                return $false;
            }
        }
    
        New-PSDrive -PSProvider Registry -Name HKU -Root Registry::HKEY_USERS | Out-Null
    
        return $true
    }
}