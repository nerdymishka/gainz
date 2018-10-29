

Import-Module "$PSScriptRoot\update.psm1" -Force 

if(!(Test-Checkpoint "MiniInstall"))
{
    Invoke-ChocolateyInstall "keepass"
    Invoke-ChocolateyInstall "openvpn"
    Invoke-ChocolateyInstall "vscode"
    Invoke-ChocolateyInstall "docfx"
    Invoke-ChocolateyInstall "googlechrome"
    Invoke-ChocolateyInstall "git-credential-manager-for-windows"
    Invoke-ChocolateyInstall "nodejs-lts"
    choco pin add -n="nodejs-lts"
    Invoke-ChocolateyInstall "adobereader"
    Invoke-ChocolateyInstall "7zip"
    Invoke-ChocolateyInstall "adobereader"
    Invoke-ChocolateyInstall "dotnetcore-sdk"
    Invoke-ChocolateyInstall "sql-server-management-studio"
    Invoke-ChocolateyInstall "docker-for-windows"

    Save-Checkpoint "MiniInstall"
}





if (Test-PendingReboot) { Invoke-Reboot }

if(!(Test-Checkpoint "VisualStudio"))
{
    $parameters = "--params=`"'--in $PSScriptRoot/visualstudio-pro-2017.json'`""

    Invoke-ChocolateyInstall "visualstudio2017professional" -ArgumentList $parameters
    choco pin add -n="visualstudio2017professional"
    Save-Checkpoint "VisualStudio"

    if (Test-PendingReboot) { Invoke-Reboot }
}



if(!(Test-Checkpoint "SqlServer"))
{
    $pw = New-Password -Length 20 -AsString 
    $c = Get-Content "$PSScriptRoot\sqlserver-2017.ini"
    $c.Replace("{{ sa }}", $pw)
    $c | Set-Content "$PSScriptRoot\sqlserver-2017.ini" -Value $c

    $parameters = "--params=`"' /ConfigurationFile:`"$PSScriptRoot\sqlserver-2017.ini`"'`""
    
    Invoke-ChocolateyInstall "sql-server-2017" -ArgumentList $parameters
    choco pin add -n="sql-server-2017"
    Save-Checkpoint "SqlServer"

    if (Test-PendingReboot) { Invoke-Reboot }
}

if(!(Test-Checkpoint "WindowsFeatures"))
{
    Enable-WindowsOptionalFeature -Online -FeatureName Microsoft-Windows-Subsystem-Linux -All -NoRestart 

    Save-Checkpoint "WindowsFeatures"
    if (Test-PendingReboot) { Invoke-Reboot }
}


Install-WindowsUpdate -all 
if (Test-PendingReboot) { Invoke-Reboot }


choco update all -y



#Enable-WindowsOptionalFeature -Online -FeatureName Microsoft-Hyper-V -All
#Enable-WindowsOptionalFeature -Online -FeatureName Microsoft-Windows-Subsystem-Linux


