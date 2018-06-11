
if(!$PSScriptRoot) {
    $PSScriptRoot = $MyInovocation.PSScriptRoot
}

if([Type]::GetType("Newtonsoft.Json.JsonToken") -eq $Null) {
    if($PSVersionTable.PSEdition -eq "Core") {
        [System.Reflection.Assembly]::LoadFile("$PSScriptRoot\bin\netstandard1.3\Newtonsoft.Json.dll") | Out-Null
    } else {
        [System.Reflection.Assembly]::LoadFile("$PSScriptRoot\bin\portable\Newtonsoft.Json.dll") | Out-Null
    }
}

Get-Item "$PsScriptRoot\*.ps1" | ForEach-Object {
     . "$($_.FullName)"
}

Export-ModuleMember  -Function ConvertTo-NewtonsoftJson
Export-ModuleMember  -Function Get-NewtonsoftJsonSettings
Export-ModuleMember  -Function Set-NewtonsoftJsonSettings
Export-ModuleMember  -Function New-NewtonsoftJsonSettings