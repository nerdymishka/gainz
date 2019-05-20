
function Read-GzModuleSetting() {
    Param(
        [Parameter(Position = 0)]
        [String] $Path,

        [Parameter(Position = 1)]
        [Object] $DefaultValue = $null,  

        [Switch] $Force,

        [String] $Storage
    )

    if($null -ne $Path)
    {
        if(!$Path.Contains(":/")) {
            $Path = "gz:/" + $Path 
        }
    
        $index = $Path.IndexOf(":/")
        $location = $Path.Substring($index + 1)
        $fileName = $Path.Substring(0, $index)
    } else {
        $fileName = "gz"
        $location = $null;
    }


   
    $config = $null 
    $var = Get-Variable $fileName -Scope Script -EA SilentlyContinue 
    if($var)
    {
       $config = $var.Value
    } 

   
    if(!$config -or $Force.ToBool()) {
        if(!$Storage) { $Storage = "$HOME/.config/gz"}
        $file = "$Storage/$fileName.json"
        if((Test-Path $file)) {
            if($PSVersionTable.PSVersion.Major -lt 6)
            {
                $config = Get-Content $file -Raw | ConvertFrom-Json
                $config = $config | ConvertTo-Hashtable 
            } else {
                $config = Get-Content $file -Raw | ConvertFrom-Json -AsHashtable 
            }
        } else {
            $config = @{}
        }

        Set-Variable -Name $fileName -Scope Script -Value $config 
    }

    if([string]::IsNullOrEmpty($location)) {
        return $config;
    }

    $parts = $location.Split("/")
    $section = $config;

    for($i = 0; $i -lt $parts.Length; $i++)
    {
        $part = $parts[$i]
        if([string]::IsNullOrWhiteSpace($part)) {
            continue;
        }

       
        if(!$section.ContainsKey($part)) {
            return $DefaultValue;
        }

        if($i -eq ($parts.Length -1)) {
            return $section[$part];
        }

        $section = $section[$part];
    }

    return $DefaultValue;
}