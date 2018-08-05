

function Set-ModuleVariable() {
    Param(
        [String] $Name,
        [Object] $Value 
    )

    $root = Get-ModuleVariable
    $root | Add-Member -NotePropertyName $Name -NotePropertyValue $Value -Force 
}