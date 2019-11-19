
function Disable-WindowsAutoLogon() {
    Param(
        
    )

    $autoLogonPath = 'HKLM:\SOFTWARE\Microsoft\Windows NT\CurrentVersion\Winlogon'


    New-ItemProperty -Path $autoLogonPath `
        -Name AutoAdminLogon `
        -Value 0 `
        -Force

    $userName = Get-ItemProperty -Path $autoLogonPath `
        -Name DefaultUserName `
        -ErrorAction SilentlyContinue 

    if($userName) {
        Remove-ItemProperty -Path $autoLogonPath `
            -Name DefaultUserName `
            -Force 
    }

    $password =  Get-ItemProperty -Path $autoLogonPath `
        -Name DefaultPassword `
        -ErrorAction SilentlyContinue

    if($password) {
        Remove-ItemProperty -Path $autoLogonPath `
            -Name DefaultPassword `
            -Force 
    }
}

