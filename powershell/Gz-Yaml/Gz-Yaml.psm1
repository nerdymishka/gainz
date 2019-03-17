
#Add-Type "./bin/netstandard1.3/YamlDotNet.dll"


Get-Item "$PSScriptRoot/public/*.ps1" | ForEach-Object {
    . "$($_.FullName)"
}

Export-ModuleMember -Function @(
    'ConvertTo-GzYaml',
    'ConvertFrom-GzYaml',
    'Add-GzYamlAlias',
    'Remove-GzYamlAlias'
)
