Param(
    [String] $ConfigPath,

    [String] $NgixConfigName
)

Copy-Item $ConfigPath "/etc/nginx/sites-available/$NginxConfigName" -Force

chown www-data:www-data "/etc/nginx/sites-available/$NginxConfigName" 