function Step-KwehBuildContext() {
    Param(
        [PsCustomObject] $Config,
        [String] $Parameters 
    )

    [hashtable]$Params = Read-KwehParameters $Parameters 
    
    $Common = New-Object PsCustomObject -Property @{
        Destination = New-Object PsCustomObject -Property @{
            Alias = "Dest"
            Type = "Path"
        }
        Extract = New-Object PsCustomObject -Property @{
            Alias = "X"
            Type = "Flag"
        }
        Log = New-Object PsCustomObject -Property @{
            Type = "Path"
        }
        Multiple = New-Object PsCustomObject -Property @{
            Alias = "M"
            Type = "Flag"
        }
        Symlink = New-Object PsCustomObject -Property @{
            Type = "Flag"
        }
        KwehConf = New-Object PsCustomObject -Property @{
            Type = "Path"
        }
        UserInstall = New-Object PsCustomObject -Property @{
            Type = "Flag"
        }
        Usb = New-Object PsCustomObject -Property @{
            Type = "Flag"
        }
    }

    $def = $Config.Parameters 

    if($Env:ChocolateyToolsLocation) {
        $optDir = $Env:ChocolateyToolsLocation.Replace("\", "/")
    } else {
        $optDir = "/apps"
    }

    $packageDir = $null 
    $toolsDir = $null 
    if($Env:ChocolateyPackageFolder) {
        $packageDir = $Env:ChocolateyPackageFolder
    } else {
        $packageDir = $Config.PackageDir 
    }

    if(Test-Path "$packageDir/tools") {
        $toolsDir = "$packageDir/tools"
    } 

    $Context = New-Object PsCustomObject -Property @{
        Opt = $optDir
        Bin = "$optDir/bin"
        Lib = "$optDir/lib"
        Etc = "$optDir/etc"
        Var = "$optDir/var"
        Data = "$optDir/var/lib"
        Log = "$optDir/var/log"
        Home = $Home
        Local = $Env:LOCALAPPDATA
        Roaming = $Env:APPDATA 
        VarTmp = "$optDir/var/tmp"
        Tmp = "$optDir/tmp"
        Label = $Config.App
        App = $Config.App
        InstallLabel = if($Config.App) { $Config.App } else {$Config.Id }  
        Id = $Config.Id 
        Version = $Config.Version
        PackageDir = $packageDir
        ToolsDir = $toolsDir
        Destination = $null
        Is64bit = [IntPtr]::Size -eq 8
        OptInstall = if($Config.Id -match "opt-") { $true } else { $false }
        PortableInstall = if($Config.Id -match ".portable") { $true } else { $false }
        Shims = if($Config.Shims) { $Config.Shims } else { @() }
    } 

    if($Context.Is64bit -and $Env:ChocolateyForceX86) {
        $Context.Is64Bit = $false;
    }

    Write-Host "Is64Bit $($Context.Is64Bit)"

    foreach($key in $Params.Keys) {
        $Common | Get-Member -MemberType NoteProperty | ForEach-Object {
            $Name = $_.Name 
            $Alias = $Common.$Name.Alias
            $Type = $Common.$Name.Type

            if($Alias) {
 
                if($key -eq $Alias) {
                    if($Type -eq "Flag") {
                        $Context | Add-Member NoteProperty -Name $Name -Value $True 
                    } else {
                        $Context | Add-Member NoteProperty -Name $Name -Value ($Params[$key])
                    }
                }
                return
            }  
            if($Name -eq $key) {
                if($Type -eq "Flag") {
                    $Context | Add-Member NoteProperty -Name $Name -Value $True 
                } else {
                    $Context | Add-Member NoteProperty -Name $Name -Value ($Params[$key])
                }
            }
        }

        if($def) {
            $def | Get-Member -MemberType NoteProperty | ForEach-Object {
                $Name = $_.Name 
                $Alias = $Common.$Name.Alias
                $Type = $Common.$Name.Type
                if($Alias) {
                    if($key -eq $Alias) {
                        if($Type -eq "Flag") {
                            $Context | Add-Member NoteProperty -Name $Name -Value $True 
                        } else {
                            $Context | Add-Member NoteProperty -Name $Name -Value ($Params[$key])
                        }
                    }
                    return
                }  
                if($Name -eq $key) {
                    if($Type -eq "Flag") {
                        $Context | Add-Member NoteProperty -Name $Name -Value $True 
                    } else {
                        $Context | Add-Member NoteProperty -Name $Name -Value ($Params[$key])
                    }
                }
            }
        }
    }

    if($Context.OptInstall) {
        if(!$Context.Destination) {
            $opt = $Context.Opt 
            $installLabel = $Context.InstallLabel
            if($Context.Multiple) {
                $version = $Context.Version 
                $installLabel = "$installLabel.$version"
                $Context.InstallLabel = $installLabel
            }

            $Context.Destination =  "$opt/$installLabel"
        }   
    }

    return $Context
}

Set-Alias -Name Step-BuildContext -Value Step-KwehBuildContext