<#
Setups up an environment for testing portable packages.
#>

$cwd = (Resolve-Path "$PSScriptRoot/..").Path;
$testDir = "$cwd/test"

$structure = @(
    "$testDir/opt",
    "$testDir/opt/bin",
    "$testDir/opt/packages",
    "$testDir/var/tmp",
    "$testDir/var/lib",
    "$testDir/var/logs",
    "$testDir/var/sites"
    "$testDir/etc",
    "$testDir/home/test_user",
    "$testDir/home/test_user/data",
    "$testDir/home/test_user/data/local",
    "$testDir/home/test_user/data/roaming",
    "$testDir/home/test_user/apps",
    "$testDir/home/test_user/.config",
    "$testDir/home/test_user/.config/PowerShell",
    "$testDir/home/test_user/.ssh",
    "$testDir/usr",
    "$testDir/usr/lib",
    "$testDir/usr/bin"
)

foreach($dir in $structure)
{
    if(!(Test-Path $dir))
    {
        New-Item $dir -ItemType Directory 
    }
}

if(! (Test-Path "$testDir/opt/pwsh")) {

    [System.Net.ServicePointManager]::SecurityProtocol = [System.Net.SecurityProtocolType]::Tls12

    Invoke-WebRequest -uri "https://github.com/PowerShell/PowerShell/releases/download/v6.1.0-preview.2/PowerShell-6.1.0-preview.2-win-x64.zip" `
        -MaximumRedirection 20  -OutFile "$testDir/var/tmp/pwsh.zip"
    
    Expand-Archive -Path "$testDir/var/tmp/pwsh.zip" `
         -DestinationPath "$testDir/opt/pwsh"
         
    "$testDir/opt/pwsh/pwsh.exe %*" > "$testDir/opt/bin/pwsh.cmd"

    Copy-Item "$PSScriptRoot/resources/Profile.ps1" -Destination `
        "$testDir/opt/pwsh"

}

& "$testDir/opt/pwsh/pwsh.exe" -noexit -File "$PSScriptRoot/Pwsh.ps1"


