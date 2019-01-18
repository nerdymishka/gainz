function Read-GzPowershellModule() {
    
    [CmdletBinding()]
    Param()

    PROCESS {
        $mods = Get-Module -ListAvailable
        $set = @();
    
        $now = [DateTime]::UtcNow
        $epoch = ($now.Ticks - 621355968000000000) / 10000;
    
        foreach($mod in $mods) {
            $set += [PSCustomObject]@{
                name = $mod.Name 
                version = $mod.Version.ToString()
                author = $mod.author
                path = $mod.Path  
                description = $mod.description
                tags = $mod.Tags 
                repoSource = $mod.RepositorySourceLocation
                rowCreatedAt = $epoch 
                rowUpdatedAt = $epoch
                rowRemovedAt = $null 
                rowCreatedAtDisplay = $now.ToString()
                rowUpdatedAtDisplay = $now.ToString()
                rowRemovedAtDisplay = $now.ToString()
            }
        }
    
        return $set;
    }
}