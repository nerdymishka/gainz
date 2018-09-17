
function Set-GainzModuleOption() {
    Param(
        [Parameter(Position = 0)]
        [String] $Name,

        [Parameter(Position = 1, ValueFromPipeline = $true)]
        [Object] $Value,

        [Switch] $Persist,

        [Switch] $Force 
    )
    $parts = @($Name);
    if($Name.Contains(".")) {
        $parts = $Name.Split("")
    }
    if($Name.Contains("/")) {
        $parts = $Name.Split("/")
    }

    $dest = Get-GainzModuleOption -Force 
    $last = $parts.Length -1
    $f = $Force.ToBool();
    if($last -gt 0) {
        for($i = 0; $i -lt $last; $i++) {
            $part = $parts[$i];
            if($gainzModuleOptions.ContainsKey($part)) {
                $dest = $gainzModuleOptions[$part];
            } else if($f) {
                $dest = $gainzModuleOptions[$part] = ${};
            } else {
                Write-Warning "$Part in $Name not found"
                return;
            }
        }
    }
    $key = $parts[$last];
    $dest[$key] = $Value;

    if($Persist.ToBool()) {
        Write-GainzModuleOption
    }
}