$vsWherePath = $null;


function Get-VsWhereLocation() {
    Param(
        [Switch] $Force 
    )
    if($vsWherePath -and !$Force.ToBool()) {
        return $vsWherePath;
    }

    $vsWhere = Get-Command "vswhere.exe" -ErrorAction SilentlyContinue
    if(!$vsWhere) {
        $vsWhere = Get-Item "${Env:ProgramFiles(x86)}\Microsoft Visual Studio\Installer\vswhere.exe" -EA SilentlyContinue
       
        if($vsWhere -and (Test-Path $vsWhere.FullName)) {
            $vsWhere = $vsWhere.FullName
        }
    } else {
        $vsWhere = $vsWhere.Path;
    }

    $vsWherePath =$vsWhere;

    return $vsWherePath;
}