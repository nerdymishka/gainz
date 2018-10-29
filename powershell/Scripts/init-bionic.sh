#!/bin/bash

sudo apt install software-properties-common python-software-properties -y
sudo add-apt-repository universe

sudo add-apt-repository ppa:gophers/archive

sudo apt-key adv --keyserver packages.microsoft.com --recv-keys EB3E94ADBE1229CF
sudo apt-key adv --keyserver 
sudo sh -c 'echo "deb [arch=amd64] https://packages.microsoft.com/repos/microsoft-ubuntu-bionic-prod bionic main" > /etc/apt/sources.list.d/dotnetdev.list'
sudo apt update
sudo apt upgrade -y
curl https://packages.microsoft.com/keys/microsoft.asc | sudo apt-key add -

# Register the Microsoft Ubuntu repository
sudo curl -o /etc/apt/sources.list.d/microsoft.list https://packages.microsoft.com/config/ubuntu/18.04/prod.list
sudo add-apt-repository ppa:unit193/encryption -y
sudo apt-get update

sudo apt install -y apt-transport-https dotnet-sdk-2.1 cifs-utils powershell

if [ -s ~/.ssh/authorized_keys ] then
    echo "keys exist"
else 
    curl https://gitlab.com/michaelherndon.keys |  tee -a ~/.ssh/authorized_keys
fi


CMD=$(cat <<EOF

Install-PackageProvider -Name "Nuget" -Force
Install-Package "Gainz-PasswordGenerator" -Force
Install-Package "Gainz-ProtectData" -Force
Install-Package "Gainz-Yaml" -Force
Install-Package "Gainz-ResolveStache" -Force
EOF
)

pwsh -Command "& { $CMD }"