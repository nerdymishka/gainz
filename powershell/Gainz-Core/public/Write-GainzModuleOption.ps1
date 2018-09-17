function Write-GainzModuleOption() {
    [CmdletBinding()]
    Param(
        [Parameter(Position = 0)]
        [String] $Path 
    )

    Process {
        if([String]::IsNullOrWhiteSpace($Path)) {
            $Path = "$HOME/.config/gainz/config.json"
        }
    
        $dir = Split-Path $Path 
        if(!(Test-Path $dir)) {
            New-Item $dir -ItemType Directory -Force | Write-Debug 
        }
    
        $data = Get-GainzModuleOption -Force 
        $json = ConvertTo-Json $data 
        Set-Content -Path $path -Value $json -Encoding 'UTF8' -Force  | Write-Debug
    }
}