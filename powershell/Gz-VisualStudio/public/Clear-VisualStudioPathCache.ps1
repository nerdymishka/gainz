function Clear-VisualStudioPathCache() {
    Param()
    
    $vsPaths = Get-ModuleVariable "VsPaths"
    if($null -ne $vsPaths.Value) {
        Set-Variable -Name "vsPaths" -Value $Null
    }
}