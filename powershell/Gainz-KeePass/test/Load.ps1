
if($PSVersionTable.PSEdition -eq "Core") {
    
    Add-Type -Path "$PSScriptRoot\..\bin\netstandard2.0\NerdyMishka.Bits.dll"
    Add-Type -Path "$PSScriptRoot\..\bin\netstandard2.0\NerdyMishka.MagicSatchel.dll"
    Add-Type -Path "$PSScriptRoot\..\bin\netstandard2.0\NerdyMishka.GoDark.dll"
    Add-Type -Path "$PSScriptRoot\..\bin\netstandard2.0\NerdyMishka.KeePass.Core.dll"
    Add-Type -Path "$PSScriptRoot\..\bin\netstandard2.0\NerdyMishka.KeePass.Xml.dll"
} else {
    Add-Type -Path "$PSScriptRoot\..\bin\net451\NerdyMishka.Bits.dll"
    Add-Type -Path "$PSScriptRoot\..\bin\net451\NerdyMishka.MagicSatchel.dll"
    Add-Type -Path "$PSScriptRoot\..\bin\net451\NerdyMishka.GoDark.dll"
    Add-Type -Path "$PSScriptRoot\..\bin\net451\NerdyMishka.KeePass.Core.dll"
    Add-Type -Path "$PSScriptRoot\..\bin\net451\NerdyMishka.KeePass.Xml.dll"
}

Get-Item "$PsScriptRoot\..\private\*.ps1" | ForEach-Object {
    . "$($_.FullName)"
}


Get-Item "$PsScriptRoot\..\public\*.ps1" | ForEach-Object {
    . "$($_.FullName)"
}