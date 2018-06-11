function Compare-KwehSemanticVersion() {
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [string] $ExpectedVersion,
        
        [Parameter(Mandatory = $true, Position = 1)]
        [string] $Version
    )
    
    if($ExpectedVersion -eq $Version) {
        return 0;
    }
    
    $left = Read-SemanticVersion $ExpectedVersion
    $right = Read-SemanticVersion $Version;
    

    if($left["Major"] -gt $right["Major"]) {
        return 1;
    }

    if($left["Major"] -lt $right["Major"]) {
        return -1;
    }
    
    if($left["Minor"] -gt $right["Minor"]) {
        return 1;
    }

    if($left["Minor"] -lt $right["Minor"]) {
        return -1;
    }
    
    if($left["Patch"] -gt $right["Patch"]) {
       
        return 1;
    }

    if($left["Patch"] -lt $right["Patch"]) {
       
        return -1;
    }
    
    $lLabel = $left.ContainsKey("Label");
    $rLabel = $right.ContainsKey("Label")
    
    if($lLabel -ne $false -or $rLabel -ne $false) {
        if($lLabel -and -not $rLabel) {
            return -1;
        }
        
        if(-not $lLabel -and $rLabel) {
            return 1;
        }
        
        if($lLabel -and $rLabel -and ($left["Label"] -ne $right["Label"])) {
            if ($left["Label"] -gt $right["Label"]) {
                return 1;
            }
        }
    }
    
    $lBuild= $left.ContainsKey("Build");
    $rBuild = $right.ContainsKey("Build")
    
    if($lBuild -ne $false -or $rBuild -ne $false) {
        if($lBuild -and -not $rBuild) {
            return -1;
        }
        
        if(-not $lBuild -and $rBuild) {
            return 1;
        }
        
        if($lBuild -and $rBuild -and ($left["Build"] -ne $right["Build"])) {
            if ($left["Build"] -gt $right["Build"]) {
                return 1;
            }
        }
    }

    return -1;
}

Set-Alias -Name Compare-SemanticVersion -Value Compare-KwehSemanticVersion