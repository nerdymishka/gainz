function Read-KwehParameters() {
    Param(
        [Parameter(Position = 0)]
        [string] $Parameters = $null,
        [switch] $LowerCasePropertyNames
    )

    $arguments = @{};
    if([string]::IsNullOrWhiteSpace($Parameters)) {
        return $arguments;
    }


    $isProperty = $false; 
    $isPropertyName = $false;
    $isPropertyValue = $false;
    $inDoubleQuotes = $false;
    $inSingleQuotes = $false;
    $propertyName = $null;
    $lowercase = $LowerCasePropertyNames.IsPresent
    $token = "";
    
    $words = @()
   
    $prefixes = [char[]]@('-', '/');
    $delimiters = [char[]]@(':', '=');
    
    for($i = 0; $i -lt $Parameters.Length;$i++) {
        $char = $Parameters[$i]
        
        $isDoubleQuote = $char -eq '"'
        if($isDoubleQuote -and -not $inSingleQuotes) {
            $inDoubleQuotes = -not $inDoubleQuotes;
            
            if(-not $inDoubleQuotes) {
                if( $isPropertyValue) {
                    $arguments.Add($propertyName, $token);
                    $isPropertyValue = $isProperty = $false;
                } else {
                    $words += $token;
                }
            } else {
                if($isProperty) {
                    $isPropertyName = $false;
                    $isPropertyValue = $true;
                }
            }
            
            $token = "";
            continue;
        }
        
        $isSingleQuote = $char -eq ''''
        if($isSingleQuote -and -not $inDoubleQuotes) {
            $inSingleQuotes = -not $inSingleQuotes;
            
            if(-not $inSingleQuotes) {
                if( $isPropertyValue) {
                    $arguments.Add($propertyName, $token);
                    $isPropertyValue = $isProperty = $false;
                } else {
                    $words += $token;
                }
            } else {
                if($isProperty) {
                    $isPropertyName = $false;
                    $isPropertyValue = $true;
                }
            }
            
            continue;
        }
        
        if($inDoubleQuotes -or $inSingleQuotes) {
            $token += $char 
            continue;
        }
        
        if($prefixes.IndexOf($char) -gt -1) {
            $isProperty = $isPropertyName = $true;
            continue;
        }
        
        if($delimiters.IndexOf($char) -gt -1) {
            $propertyName = $token;
            
            if($lowercase) {
                $propertyName = $propertyName.ToLower();
            }
            
            $token = "";
            $isPropertyName = $false;
            $isPropertyValue = $true;
            continue;
        }
        
        if($char -eq ' ') {
            
            if($token.Length -eq 0) {
                $isProperty = $isPropertyName = $isPropertyValue = $false;
                continue;
            } 
            
            if($isPropertyName) {
                $isProperty = $isPropertyName = $false;
                if($token.Length -gt 0) {
                    if($lowercase) {
                        $token = $token.ToLower();
                    }
                    $arguments.Add($token, $true);
                    $token = ""
                    continue;
                }
            }
            
            if($isPropertyValue) {
                $isProperty = $isPropertyValue = $false;
                if($token.Length -gt 0) {
                    $arguments.Add($propertyName, $token);
                    $token = "";
                    continue;
                }
            }
            
            if($token.Length -gt 0) {
                $words += $token;
                $token = "";
                continue;
            }
        }

        $token += $char;
    }
    
    if($inDoubleQuotes) {
        Write-Error "Parse Error: A closing double quote is missing from your argument string";
        return $arguments;
    }
    
    if($inSingleQuotes) {
        Write-Error "Parse Error: A closing single quote is missing from your argument string."
        return $arguments;
    }
    
    if($token.Length -gt 0) {
        if($isPropertyName) {
            $arguments.Add($token, $true);
        }
        elseif($isPropertyValue) {
            $arguments.Add($propertyName, $token);
        } else {
            $words += $token;
        }
    }
   
    foreach($word in $words) {
        $arguments.Add($word, $true);
    }

    return [hashtable]$arguments;
}