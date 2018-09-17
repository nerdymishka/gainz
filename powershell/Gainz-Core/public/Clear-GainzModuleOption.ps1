function Clear-GainzModuleOption() {
    [CmdletBinding()]
    Param(
        [Switch] $Force,
        [String] $Path 
    )

    Process {
        if($Force.ToBool()) {
            if([String]::IsNullOrWhiteSpace($Path)) {
                $Path = "$HOME/.config/gainz/config.json"
            }

            if(Test-Path $Path) {
                Remove-Item $Path -Force | Write-Debug
                return !(Test-Path $Path)
            }

            return $True;
        }
    }
}