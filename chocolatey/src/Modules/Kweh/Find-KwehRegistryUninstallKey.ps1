function Find-KwehRegistryUninstallKey() {
    
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [string] $DisplayName,
        [string] $Comparison = "eq" 
    )
    
    Write-Host $packageName
    
    $keyNames = @(
        "HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
        "HKLM:\SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall",
        "HKLM:\SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall"
    )

    
    foreach($keyName in $keyNames) {
        
        
        $key = Get-Item $keyName -ErrorAction SilentlyContinue
        if($key -eq $null) {
            continue;
            # TODO: add Write-Debug
        }

        $subKeyNames = $key.GetSubKeyNames();
        
        foreach($shortKeyName in $subKeyNames) {
             $subKeyName = "$keyName\$shortKeyName"
             $subKey = Get-Item $subKeyName -ErrorAction SilentlyContinue
             
             if($subKey -ne $null) {
                
                $currentDisplayName = $subKey.GetValue("DisplayName");
                if($currentDisplayName -eq $Null) {
                    continue;
                }
                $match = $false 
                switch($Comparison) {
                    "eq" { $match = $DisplayName -eq $currentDisplayName}
                    "sw" { $match = $DisplayName.ToLower().StartsWith($currentDisplayName.ToLower())}
                    "contains"  {$match = $currentDisplayName -match $DisplayName}
                    default { $match = $DisplayName -match $currentDisplayName}
                }
                
                if($match) {
                    return $subKey;
                } 
             }
        }
    }
    
    return $null;
}

Set-Alias -Name Find-RegistryUninstallKey -Value Find-KwehRegistryUninstallKey 