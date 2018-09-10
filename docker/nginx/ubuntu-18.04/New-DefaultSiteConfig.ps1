Param(
    [Parameter(Position = 0)]
    [String] $Template,

    [Parameter(Position = 1)]
    [String] $ServerName = "localhost",

    [Parameter(Position = 2)]
    [String] $CertificateName,

    [Switch] $GenerateCert,

    [Switch] $GenerateDhParam,

    [String] $Country = "US",
    
    [string] $Region = "DC",
    
    [string] $Location = "DC",
    
    [string] $Organization = "Developer",
    
    [string] $Department = "Developer",
    
    [string] $Subject = "localhost",
    
    [string] $ConfigName = "default"
)

if([string]::IsNullOrWhiteSpace($CertificateName)) {
    $CertificateName = $ServerName
}

if($GenerateDhParam.ToBool()) {
    Write-Host "grab some coffee";
    . "/New-DhParam.ps1"
}

if($GenerateCert.ToBool()) {
    $key = "/etc/nginx/certs/$CertificateName.key"
    $keyPass = "/etc/nginx/certs/$CertificateName.pass.key"
    $csr = "/etc/nginx/certs/$CertificateName.csr"
    $crt = "/etc/nginx/certs/$CertificateName.crt"
    openssl genrsa -des3 -passout pass:x -out $keyPass 2048
    openssl rsa -passin pass:x -in $keyPass -out $key
    rm $keyPass
    openssl req -new -key $key -out $csr \
        -subj "/C=$Country/ST=$State/L=$Region/O=$Organization/OU=$Department/CN=$Subject"
    openssl x509 -req -days 365 -in $csr -signkey $key  -out $crt

    chown www-data:www-data $key 
    chown www-data:www-data $crt 
}

$content = $null;
$model = [PSCustomObject]@{
    serverName = $ServerName
    certificateName = $CertificateName
}

. "/Resolve-Stache.ps1"
switch($Template) {
    "dotnet" {
        $content = Get-Content "/etc/nginx/samples/dotnet.conf" -Raw
        $content = Resolve-Stache -Template $content -Model $model  
    }
    "dotnet-test" {
        $content = Get-Content "/etc/nginx/samples/dotnet-test.conf" -Raw
        $content = Resolve-Stache -Template $content -Model $model  
    }
    "php-fpm" {
        $content = Get-Content "/etc/nginx/samples/php-fpm.conf" -Raw
        $content = Resolve-Stache -Template $content -Model $model  
    }
    "php-fpm-test" {
        $content = Get-Content "/etc/nginx/samples/php-fpm-test.conf" -Raw
        $content = Resolve-Stache -Template $content -Model $model  

        if(!(Test-Path ("/var/www/html/index.php"))) {
            "<?php phpinfo() ?>" | Out-File "/var/www/html/index.php"
        }
    }
    default  {
        Write-Error "unknown template $Template"
    }
}

if(Test-Path "/etc/nginx/sites-available/$ConfigName") {
    Remove-Item  "/etc/nginx/sites-available/$ConfigName" -Force
}

$content | Out-File "/etc/nginx/sites-available/$ConfigName"

chown www-data:www-data "/etc/nginx/sites-available/$ConfigName"