

function Write-GzModuleSetting() {
    Param(
        [Parameter(Position = 0)]
        [String] $Path,

        [Parameter(Position = 1)]
        [Object] $Value,

        [String] $Storage = $null
    )

    if(!$Path.Contains(":/")) {
        $Path = "gz:/" + $Path 
    }

    $index = $Path.IndexOf(":/")
    $location = $Path.Substring($index + 2)
    $fileName = $Path.Substring(0, $index)
    $config = @{}
    $var = Get-Variable $fileName -Scope Script -EA SilentlyContinue 
    if($var) {
       $config = $var.Value
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
            $section.Add($part, @{})
        }

        if($i -eq ($parts.Length -1)) {
            $section[$part] = $Value
            break;
        }

        $section = $section[$part];
    }

    if(!$Storage) { $Storage = "$HOME/.config/gz" }
    if(!(Test-Path $Storage)) {
        New-Item $Storage -ItemType Directory | Write-Debug
    }

    $config | ConvertTo-Json -Depth 10 | Out-File "$Storage/$filename.json" -Encoding "UTF8"
    Set-Variable -Name $fileName -Scope Script -Value $config 
}