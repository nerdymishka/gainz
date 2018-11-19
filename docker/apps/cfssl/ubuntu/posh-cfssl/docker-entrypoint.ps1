Import-Module "$PSScriptRoot/posh-cfssl.psm1" -Force

Write-Host $args

if($args[0] -eq "cfssl") {
    Write-Host "Hello World" -ForegroundColor Green
  
} else {

}

$exe = $args[0];
$argz = @();
for($i = 1; $i -lt $args.Length; $i++) {
    $argz += $args[$i];
}

$cfg = Read-Config 

if($args[0] -eq "cfssl") {
    $argz += "-config"
    $argz += $cfg.config 
    $argz += "-db-config"
    $argz += $cfg.db 
}


Initialize-CfsslServer 

Write-Host "/var/lib/cfssl"
& ls "/var/lib/cfssl/"

Write-Host "/etc/cfssl"
& $exe @argz