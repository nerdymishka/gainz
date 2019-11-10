
function Enable-WindowsAutoLogon() {
    Param(
        [Parameter(Mandatory = $true)]
        [PsCredential] $Credential 
    )

    $autoLogonPath = 'HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon'

    New-ItemProperty -Path $autoLogonPath `
        -Name AutoAdminLogon `
        -Value 1 `
        -Force

    New-ItemProperty -Path $autoLogonPath `
        -Name DefaultUserName `
        -Value $Credential.UserName `
        -Force 

    New-ItemProperty -Path $autoLogonPath `
        -Name DefaultPassword `
        -Value ($Credential.GetNetworkCredential().Password) `
        -Force
}

