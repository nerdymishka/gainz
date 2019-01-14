function Read-GzWinBitLockerStatus() {
    
    [CmdletBinding()]
    Param(
        
    )

    
    PROCESS {
        $volumes = Get-Volume
        $set = @();

        foreach($vol in $volumes)
        {
            $drive = $vol.DriveLetter;
            if(!$drive) {
                continue;
            }   
            $v = Get-BitLockerVolume $drive -ErrorAction SilentlyContinue 

            if(!$v) {
                continue;
            }
        
            $pw = $false;
            foreach($k in $v.KeyProtector) {
                if($k.KeyProtectorType -eq "RecoveryPassword") {
                    $pw = $null -ne $k.RecoveryPassword
                    break; 
                }
            }

            $now = [DateTime]::UtcNow
            $epoch = ($now.Ticks - 621355968000000000) / 10000;

            $set += [PSCustomObject]@{
                drive = $v.MountPoint
                method = $v.EncryptionMethod
                encryptionType = "bitlocker"
                status = $v.protectionStatus
                recoveryKey = $pw 
                rowCreatedAt = $epoch 
                rowUpdatedAt = $epoch
                rowRemovedAt = $null 
                rowCreatedAtDisplay = $now.ToString()
                rowUpdatedAtDisplay = $now.ToString()
                rowRemovedAtDisplay = $null
            }     
        }

    
        return $set;
    }
}