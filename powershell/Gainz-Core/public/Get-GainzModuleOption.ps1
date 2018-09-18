$gainzModuleOptions = @{}
$firstRead = $false;

function Get-GainzModuleOption() {
    Param(
        [Parameter(Position = 0)]
        [String] $Name,
        
        [Switch] $Force 
    )

    if(!$firstRead) {
        Read-GainzModuleOption 
    }

    if([String]::IsNullOrWhiteSpace($Name) -and $Force.ToBool()) {
        return $gainzModuleOptions;
    }

    $parts = @($Name);
    if($Name.Contains(".")) {
        $parts = $Name.Split("")
    }
    if($Name.Contains("/")) {
        $parts = $Name.Split("/")
    }

    if($parts.Length -eq 1) {
        return $gainzModuleOptions[$Name];
    }

    $dest = $gainzModuleOptions
    $last = $parts.Length -1
    $f = $Force.ToBool();
    if($last -gt 0) {
        for($i = 0; $i -lt $last; $i++) {
            $part = $parts[$i];
            if($gainzModuleOptions.ContainsKey($part)) {
                $dest = $gainzModuleOptions[$part];
            } elseif($f) {
                $dest = $gainzModuleOptions[$part] = @{};
            } else {
                Write-Warning "$Part in $Name not found"
                return;
            }
        }
    }

    $key = $parts[$last];
    return $dest[$key];
}