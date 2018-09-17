function Get-LinuxGroupId() {
    Param(
        [String] $Group 
    )

    $nonInteractive = Test-Noninteractive
    if($nonInteractive) {
        if([string]::IsNullOrWhiteSpace($Group)) {
            throw [System.ArgumentNullException] "-Group"
            return;
        }

        While([string]::IsNullOrWhiteSpace($Group)) {
            $group = Read-Host -Prompt "Group name"
        }
    }
   

    $info = getent group $Group
    if(!$info) {
        return $null;
    }
    $data = $info.Split(":");
    if($data.Length -lt 3) {
        return $null;
    }
    return $data[2]; 
}


