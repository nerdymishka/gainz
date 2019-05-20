

function Test-UnixUserIsInRole() {
    Param(
        [String] $Identity = $null,
        
        [String] $Group = "root"
    )

   

    if($null -eq $Identity) {
        $uid = id -u
        $gid = id -g 
    } else {
        $uid = id -u -n $Identity
        $gid = id -g -n $Identity
    }

    if($Group -eq "root") {
        return $uid -eq 0 -or $gid -eq 0;
    }

    throw "Not Currently Supported"
}