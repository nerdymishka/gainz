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

            $method = $v.EncryptionMethod
            $status = $v.ProtectionStatus 
            if($null -ne $method) { $method = $method.ToString() }
            if($null -ne $status) { $status = $status.ToString() }

            $set += [PSCustomObject]@{
                drive = $v.MountPoint
                method = $method 
                encryptionType = "bitlocker"
                status = $status
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