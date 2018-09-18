function Read-GainzModuleOption() {
    [CmdletBinding()]
    Param(
        [String] $Path
    )

    Process {
        if([String]::IsNullOrWhiteSpace($Path)) {
            $Path = "$HOME/.config/gainz/config.json"
        }
    
        if((Test-Path $Path)) {
            $data = Get-Content $Path -Raw 
            $Json = ConvertFrom-Json $data
            $gainzModuleOptions = $Json;
        }
    }
}