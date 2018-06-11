function Install-HkuDrive() {
    if(-not (Test-Path hku:)) {
         New-PSDrive -Name HKU -PSProvider Registry -Root Registry::HKEY_USERS
    }
} 

Set-Alias -Name Install-HkuDrive -Value Install-KwehHkuDrive 