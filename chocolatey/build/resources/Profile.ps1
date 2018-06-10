<#
    This file is copied to {project}/chocolatey/test/opt/pwsh
    and it is loaded whenever pwsh.exe is called from the
    same directory.
#> 
[System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("PSUseDeclaredVarsMoreThanAssignments", "", Target="*")]
Param()
$cwd = (Resolve-Path "$PSScriptRoot/../..").Path.Replace("\", "/")
$homeDir = "$cwd/home/test_user";


if($Env:OS -eq "Windows_NT")
{
    $Env:ORIGINAL_USER = $Env:USERNAME
    $Env:ORIGINAL_HOME = $HOME 

    Remove-Variable "HOME" -Force
    Set-Variable "HOME" $homeDir 
   
    $profilePath = "$Home/.config/PowerShell/Microsoft.PowerShell_profile.ps1"
    Remove-Variable "Profile" -Force
    Set-Variable "Profile" $profilePath

  
  
    $testDir = "$cwd/test"
    $Env:USERPROFILE = $HOME  
    $Env:APPDATA = "$HOME/data/roaming"
    $Env:LOCALAPPDATA = "$HOME/data/local"
    $Env:USERNAME = "test_user"
    $Env:ProgramData = "$HOME/var/lib"
    $env:TMP = "$testDir/var/tmp"
    $env:TEMP = "$testDir/var/tmp"
    $env:PUBLIC = "$testDir/opt"
    $x = Get-Item "HKCU:\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\User Shell Folders" 
    $Env:USER_DOCUMENTS = ("$Home\" +(Split-Path -Leaf $x.GetValue("Personal")))
    $Env:USER_DOWNLOADS = ("$Home\Downloads")
    $Env:USER_DESKTOP = ("$Home\" + (Split-Path -Leaf $x.GetValue("Desktop")))
    Set-PSReadlineOption -HistorySavePath "$HOME/data/roaming/history.txt"


    $path = "$Env:windir\system32;$Env:windir;$Env:windir\System32\Wbem"
    $path += ";$Env:windir\System32\WindowsPowerShell\v1.0\"
    $path += ";$testDir/opt/bin"
    $path += ";$testDir/usr/bin"
    $path += ";$testDir/opt/chocolatey/bin"
    $path += ";$testDir/opt/pwsh"

    [Environment]::SetEnvironmentVariable("Path", $path)
    $env:Path = $path;
}

$env:ChocolateyInstall = "$testDir/opt/chocolatey"
$env:ChocolateyToolsLocation = "$testDir/opt/packages"
