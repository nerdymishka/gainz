function Test-KwehIsElevated() {
    [cmdletbinding()]
    Param()

    if($Env:OS -eq "Windows_NT") {
        $identity = [Security.Principal.WindowsIdentity]::GetCurrent([Security.Principal.TokenAccessLevels]'Query,Duplicate')
        $currentPrincipal = New-Object Security.Principal.WindowsPrincipal($identity)
        return $currentPrincipal.IsInRole([Security.Principal.WindowsBuiltInRole]::Administrator)
    } else {
        $results = sudo env |grep SUDO
        foreach($result in $results) {
            if($result -match "SUDO_COMMAND") {
                return $true;
            }
        }
        return $false
    }

}

Set-Alias -Name Test-IsElevated -Value Test-KwehIsElevated