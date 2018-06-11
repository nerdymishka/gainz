function Read-KwehSemanticVersion() {
    Param(
        [Parameter(Position = 0, Mandatory = $true)]
        [string] $version
    )
    
    $parts = @{};
    $keys = @{0 = "Major"; 1 = "Minor"; 2 = "Patch"; 3 = "Build"; 4 = "4"; 5 = "5"}
    $segments = $version.Split(".");
    $i = 0;

    for(; $i -lt $segments.Length; $i++) {
        $value = $segments[$i];

        if($value.Contains("-")) {    
            $label = $value.Substring($value.IndexOf("-") + 1);
            $value = $value.Substring(1, $value.IndexOf("-"));
            
            $parts.Add("Label", $label);
        }

        if($value.Contains("+")) {
            $label = $value.Substring($value.IndexOf("+") + 1);
            $value = $value.Substring(1, $value.IndexOf("+"));
            
            $parts.Add("Build", $label);
        }
        
        $key = $keys[$i];
        
        if(![string]::IsNullOrWhitespace($key)) {
            
            [int] $j = 0;
            if([System.Int32]::TryParse($value, [ref]$j) ) {
                $parts.Add($keys[$i], $j);
                continue;
            }
            $parts.Add($keys[$i], $value);
        }
    }
    
    return $parts;
}

Set-Alias -Name Read-SemanticVersion -Value Read-KwehSemanticVersion 