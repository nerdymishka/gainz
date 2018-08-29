

if($PSVersionTable.PSEdition -eq "Core") {
    
    Add-Type -Path "$PSScriptRoot\bin\netstandard2.0\NerdyMishka.Bits.dll"
    Add-Type -Path "$PSScriptRoot\bin\netstandard2.0\NerdyMishka.MagicSatchel.dll"
    Add-Type -Path "$PSScriptRoot\bin\netstandard2.0\NerdyMishka.GoDark.dll"
    Add-Type -Path "$PSScriptRoot\bin\netstandard2.0\NerdyMishka.KeePass.Core.dll"
    Add-Type -Path "$PSScriptRoot\bin\netstandard2.0\NerdyMishka.KeePass.Xml.dll"
} else {
    Add-Type -Path "$PSScriptRoot\bin\net451\NerdyMishka.Bits.dll"
    Add-Type -Path "$PSScriptRoot\bin\net451\NerdyMishka.MagicSatchel.dll"
    Add-Type -Path "$PSScriptRoot\bin\net451\NerdyMishka.GoDark.dll"
    Add-Type -Path "$PSScriptRoot\bin\net451\NerdyMishka.KeePass.Core.dll"
    Add-Type -Path "$PSScriptRoot\bin\net451\NerdyMishka.KeePass.Xml.dll"
}




if(!$PSScriptRoot) {
    $PSScriptRoot = $MyInovocation.PSScriptRoot
}

if(!$PSScriptRoot) {
    $PSScriptRoot = Split-Path $MyInovocation.MyCommand.Path
}

Get-Item "$PSScriptRoot\public\*.ps1" | ForEach-Object {
    . "$($_.FullName)"
}

Get-Item "$PSScriptRoot\private\*.ps1" | ForEach-Object {
    . "$($_.FullName)"
}

$functions  = @(
    'New-KeePassKey',
    'Open-KeePassPackage',
    'Find-KeePassEntryByTitle',
    'Find-KeePassEntry',
    'New-KeePassPackage',
    'New-KeePassEntry',
    'Merge-KeePassGroups',
    'Open-KeePassRdpLink',
    'Export-KeePassBinary',
    'Save-KeePassPackage'
)
foreach($func in $functions) {
    Export-ModuleMember -Function $func
}