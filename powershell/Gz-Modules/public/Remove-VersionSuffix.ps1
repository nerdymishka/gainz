
function Remove-VersionSuffix(){
    Param(
        [String] $Version
    )

    if($null -eq $Version) { return $Null }

    if($version -match "-") {
        return $version.Substring(0, $version.IndexOf("-"))
    }

    return $version;
}