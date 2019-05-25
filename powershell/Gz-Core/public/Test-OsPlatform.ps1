

Function Test-OsPlatform() {
    Param(
        [Parameter(Position = 0)]
        [ValidateSet("Windows", "Unix", "Linux", "Mac", "Win32NT", "Win32Windows", "Win32S", "WinCE", "Xbox", "MacOSX")]
        [String[]] $Type
    )

    PROCESS {
        if(!$Type) {
            $Type = @("Windows")
        }

        $r = Get-OsPlatform
        
        foreach($t in $Type)
        {
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
    
            if($r -eq $Type) {
                return $true;
            }
        }

        return $false;
      
    }
}