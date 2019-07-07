function Read-NuspecConfig() {
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $true)]
        [String] $Path,

        [String] $Version 
    )

    if(-not (Test-Path $Path)) {
        throw [System.IO.FileNotFoundException] $Path 
    }

    $isYaml = $Path.EndsWith(".yml") -or $Path.EndsWith(".yaml")

    if($isYaml) {
        $cfg = Get-Content  $Path -Raw | ConvertFrom-Yaml
    } else {
        $cfg = Get-Content  $Path -Raw | ConvertFrom-Json 
    }
  

   

    $versionedConfigPath = $null
    if([string]::IsNullOrWhitespace($Version)) {

        if($isYaml) {
            if($Path.EndsWith(".yml")) {
                $search = $Path.Replace(".yml", "*.yml")
            } else {
                $search = $Path.Replace(".yaml", "*.yaml")
            }
            $versions = Get-Item $search | Select-Object -ExpandProperty FullName
        } else {
            $versions = Get-Item ($Path.Replace(".json","*.json")) | Select-Object -ExpandProperty FullName
        }
        
       
        
        if(($versions -is [array])) {
     
            $ranked =  Get-NuspecSemanticVersionRank $versions -IsFileName | Sort-Object Rank -Descending
            
            if($ranked.Length -gt 1) {
                
                $versionedConfig = $ranked[0];
                $versionedConfigPath = ($versionedConfig.Version)
            }
        } 
    } else  {

       $versionedConfigPath = ($Path.Replace(".json", ".$Version.json"))  
    }
    
    $versionedConfig = Get-Item $versionedConfigPath -EA SilentlyContinue
    $files = @();
    
    if($null -ne $versionedConfig -and (Get-Item $Path).FullName -ne $versionedConfig.FullName) {
        $files += $versionedConfig
    }
     
    
    for($i = 0; $i -lt $files.Length; $i++) {

        $file = $files[$i]
        $nextJson = Get-Content $file -Raw | ConvertFrom-Json
        $cfg = Merge-NuspecConfig -Source $nextJson -Destination $cfg 
    }

    return $cfg;  
}