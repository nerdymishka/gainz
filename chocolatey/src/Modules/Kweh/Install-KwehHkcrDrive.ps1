function Install-KwehHkcrDrive() {
    if(-not (Test-Path hkcr:)) {
         New-PSDrive -Name HKCR -PSProvider Registry -Root Registry::HKEY_CLASSES_ROOT
    }
}
Set-Alias -Name Install-HkcrDrive -Value Install-KwehHkcrDrive 