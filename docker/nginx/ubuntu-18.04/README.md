# nerdymishka/ngninx-ubuntu-18.04

Provides an nginx image for ubuntu-18.04 that includes powershell for scripting purposes
and sample config templates. 

### New-DefaultSiteConfig

- **Template** Required. Copies a sample config and replaces the variables
  - dotnet-test
  - dotnet
  - nodejs
  - php-fpm-test
  - php-fpm
- **ServerName** Optional. Sets the `server_name` value in the config. Defaults to "localhost". 
- **CertificateName** Optional. Sets the file name for the cert. Certs are expected to exist
  in `/etc/nginx/certs` Defaults to ServerName.
- **GenerateCert** Optional. If present, the script will generate a self-signed cert
  to `/etc/nginx/certs` which is a mountable directory.
- **GenerateDhParam** Optional. If present, this script will generate Diffie-Hellman parameters file
  which will take a while. 
- **ConfigName** Optional. If present, this will set the nginx config name that will be save to `sites-available`.
- **Country** Optional. Sets the county value for the certificate. Defaults to "US".
- **Region** Optional. Sets the county value for the certificate. Defaults to "DC".
- **Location** Optional. Sets the county value for the certificate. Defaults to "DC".
- **Department** Optional. Sets the county value for the certificate. Defaults to "Developer".
- **Organization** Optional. Sets the county value for the certificate. Defaults to "Developer".
- **Subject** Optional. Sets the county value for the certificate. Defaults to "localhost".