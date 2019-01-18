function Read-GzWinAppXPackage() {
    [CmdletBinding()]
    Param()


    PROCESS 
    {
  
        $packages = Get-AppxPackage

        $set = @();
    
        $now = [DateTime]::UtcNow
        $epoch = ($now.Ticks - 621355968000000000) / 10000;
    
        foreach($p in $packages) {
            $set += [PsCustomObject]@{
                name = $p.Name
                version = $p.Version.ToString()
                path = $p.InstallLocation 
                author = $p.Publisher 
                status = $p.Status 
                rowCreatedAt = $epoch 
                rowUpdatedAt = $epoch
                rowRemovedAt = $null 
                rowCreatedAtDisplay = $now.ToString()
                rowUpdatedAtDisplay = $now.ToString()
                rowRemovedAtDisplay = $null
            }
        }
   

        return $set 
    }
}