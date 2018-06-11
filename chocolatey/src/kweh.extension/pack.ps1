Param(
    [Parameter(Position = 0, Mandatory = $true)]
    [String] $Version, 

    [String] $Destination 
)

Remove-Item "$PSScriptRoot/tools/Kweh/*" -Force -Recurse
Copy-Item "$PSScriptRoot/../Modules/Kweh/*" "$PSScriptRoot/tools/Kweh" -Force -Recurse 

if(Test-Path "$PSScriptRoot/tools/Kweh/Kweh.psd1") {
    Remove-Item "$PSScriptRoot/tools/Kweh/Kweh.psd1" -Force  
}

if([string]::IsNullOrWhiteSpace($Destination)) {
    choco pack "$PSScriptRoot/kweh.extension.nuspec" --packageversion $Version
} else {
    choco pack "$PSScriptRoot/kweh.extension.nuspec" --packageversion $Version --out $Destination
}
 