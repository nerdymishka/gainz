#Import-Module "$PSScriptRoot/posh-cfssl.psm1" -Force

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

& $exe @argz