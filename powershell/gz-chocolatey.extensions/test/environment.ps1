
$installArgs =  [PsCustomObject]@{
    "chocolatey" = [PsCustomObject]@{
        "install" = "$PsScriptRoot/ProgramData",
        "upgrade" = "$true",
        "packages" = @(
            "chocolatey-core.extension",
            "chocolatey-windows.extension"
        )
    }
}

$h = $env:Home = "$PsScriptRoot/profile"
$env:AllUsersProfile = "$PsScriptRoot/opt" 
$env:LOCALAPPData = "$h/AppData/Local"
$env:AppData  = "$h/AppData/Roaming"
$env:ProgramFiles = "$h/opt/programFiles"
${env:ProgramFiles(x86)} = "$h/opt/programFiles32"
$env:Temp = "$h/opt/tmp"