
$wd = "$PSScriptRoot/../.."

if(Test-Path ./modules) {
    Remove-Item ./modules -Force -Recurse
}

New-Item modules -ItemType Directory 

Copy-Item  "$wd/Gainz-PasswordGenerator" "./modules" -Recurse

docker build .