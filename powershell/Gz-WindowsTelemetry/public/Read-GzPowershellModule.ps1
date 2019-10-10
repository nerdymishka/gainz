function Read-GzPowershellModule() {
    
    [CmdletBinding()]
    Param()

    PROCESS {
        $mods = Get-Module -ListAvailable
        $set = @();
    
        $now = [DateTime]::UtcNow
       
    
        foreach($mod in $mods) {
            $set += [PSCustomObject]@{
                name = $mod.Name 
                version = $mod.Version.ToString()
                author = $mod.author
                path = $mod.Path  
                description = $mod.description
                tags = $mod.Tags 
                repoSource = $mod.RepositorySourceLocation
                createdAt = $now  
            }
        }
    
        return $set;
    }
}