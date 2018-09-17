function Get-LinuxUserId() {
    Param(
        [Parameter(Position = 0)]
        [String] $User
    )

   
    if([string]::IsNullOrWhiteSpace($User)) {
        $noninteractive = Test-Noninteractive
        if($noninteractive) {
            throw [System.ArgumentNullException] "-User"
            return;
        }

        While([string]::IsNullOrWhiteSpace($User)) {
            $User = Read-Host -Prompt "User"
        }
    } 

    $r =  id -u $User
    if($r -match "no such user") {
        return $null;
    }

    return $r;
}