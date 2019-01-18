function Read-GzWinEnabledFeature() {
    
    [CmdletBinding()]
    Param()

    PROCESS {
        $wfs = Get-WindowsOptionalFeature -Online | Where-Object { $_.State -eq "Enabled" }
        $set = @() 
        $now = [DateTime]::UtcNow
        $epoch = ($now.Ticks - 621355968000000000) / 10000;

        foreach($wf in $wfs)
        {
            $set += [PsCustomObject]@{
                name = $wf.FeatureName
                log = $wf.LogPath
                logLevel = $wf.LogLevel
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