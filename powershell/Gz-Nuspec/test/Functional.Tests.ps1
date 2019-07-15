Import-Module "$PsScriptRoot/../Gz-Nuspec.psm1" -Force


Describe ".\Gz-Nuspec" {

    IT "should convert a nuspec.json file to nuspec" {
        Invoke-NuspecTransform -Path "$PsScriptRoot/nginx.portable.nuspec.json" -Chocolatey 
    }
}