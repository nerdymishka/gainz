function Read-KwehJsonConfig() {
    Param(
        [Parameter(Position = 0, ValueFromPipeline = $true)]
        [String] $Path,

        [String] $Version 
    )

    if(-not (Test-Path $Path)) {
        throw [System.IO.FileNotFoundException] $Path 
    }

    if(!$Path.EndsWith(".json")) {
        $Path += "/kweh.json"
    }

    Write-Host $Path
    

    $cfg = Get-Content  $Path -Raw | ConvertFrom-Json
    $db = Get-Content ($Path.Replace(".json", ".versions.json")) -Raw | ConvertFrom-Json
    
    if([string]::IsNullOrWhitespace($Version)) {
        
        $versions = $db | Get-Member -MemberType NoteProperty | Select-Object -ExpandProperty Name
        Write-Host $versions
        $ranked =  Get-SemanticVersionRank $versions  | Sort-Object Rank -Descending
        Write-Host $ranked
        $rank = $ranked[0]; 
        Write-host $rank
        $version = $rank.Version;
    } 
    
    $data = $db.$version
    Write-Host $data

    $cfg = Merge-KwehConfig -Source $data -Destination $cfg
    
    return $cfg;  
}