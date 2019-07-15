
function Get-InstallMsBuildPath()
{

    $subKeys = Get-ChildItem "HKLM:\SOFTWARE\Microsoft\MSBuild\ToolsVersions" -EA SilentlyContinue
    if($subKeys)
    {
        $names = $subKeys.Name
    
    }
}