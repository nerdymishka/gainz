function Clear-VisualStudioPathCache() {
    Param()
    
    $vsPaths = Get-ModuleVariable "VsPaths"
    if($vsPaths.Value -ne $null) {
        Set-Variable -Name "vsPaths" -Value $Null
    }
}