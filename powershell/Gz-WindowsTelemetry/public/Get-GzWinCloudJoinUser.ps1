function Get-GzWinCloudJoinUser() {
    [CmdletBinding()]
    Param()

    PROCESS {
        $subKey = Get-Item "HKLM:/SYSTEM/CurrentControlSet/Control/CloudDomainJoin/JoinInfo"

        $guids = $subKey.GetSubKeyNames()

        $now = [DateTime]::UtcNow
        $epoch = ($now.Ticks - 621355968000000000) / 10000;
        
        
        foreach($guid in $guids) {
            $guidSubKey = $subKey.OpenSubKey($guid);
            $user = @{
                tenantId = $guidSubKey.GetValue("TenantId");
                email = $guidSubKey.GetValue("UserEmail")
                rowCreatedAt = $epoch 
                rowUpdatedAt = $epoch
                rowRemovedAt = $null 
                rowCreatedAtDisplay = $now.ToString()
                rowUpdatedAtDisplay = $now.ToString()
                rowRemovedAtDisplay = $null 
            }
            
            $set += $user 
        }
        
        return $set 
    
    }
}