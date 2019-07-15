function Read-VsWherePathData()
{
    $vsWhere = Get-VsWhereVersion
    if(!$vsWhere) { return @{} }

    $vsPaths = @{}

    & $vsWhere -format json 
    $versionJson = $versionJson | ConvertFrom-Json
    if($versionJson -and $versionJson.Length) {
        foreach($v in $versionJson) {
                
            if($v.installationVersion) {
                $major = $v.ToString()
                $major = $major.Substring(0, $major.IndexOf("."))
                $entry = "$major.0";
                if($v.isPrerelease) {
                    $entry += "-pre"
                }
                $vsPaths.Add($entry, $v.installationPath);
            }
        }
    }

    return $vsPaths
}