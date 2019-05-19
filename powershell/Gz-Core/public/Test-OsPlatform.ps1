

Function Test-OsPlatform() {
    Param(
        [Parameter(Position = 0)]
        [ValidateSet("Windows", "Unix", "Linux", "Mac", "Win32NT", "Win32Windows", "Win32S", "WinCE", "Xbox", "MacOSX")]
        [String] $Type = "Win32NT"
    )

    PROCESS {
        $r = Get-OsPlatform  
        if($Type -eq "Windows") {
            if($r.StartsWith("Win")) {
                return $true;
            }
        }
        if($type -eq "Linux") {
            return $r -eq "Unix";
        }

        if($Type -eq "Mac") {
            if($r.StartsWith("Mac")) {
                return $true;
            }
        }

        return $r -eq $Type;
    }
}