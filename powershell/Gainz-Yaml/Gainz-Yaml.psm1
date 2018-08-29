
if($null -eq  [Type]::GetType("YamlDotNet.RepresentationModel.YamlScalarNode")) {
    if($PSVersionTable.PSEdition -eq "Core") {
        [System.Reflection.Assembly]::LoadFile("$PSScriptRoot\bin\netstandard1.3\YamlDotNet.dll") | Out-Null
    } else {
        [System.Reflection.Assembly]::LoadFile("$PSScriptRoot\bin\net45\YamlDotNet.dll") | Out-Null
    }
}

Get-Item "$PsScriptRoot\..\public\*.ps1" | ForEach-Object {
    . "$($_.FullName)"
}

Export-ModuleMember -Function @(
    'ConvertTo-Yaml',
    'ConvertFrom-Yaml'
)
