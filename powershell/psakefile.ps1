Task "docker:pull" {
    # docker pull ubuntu:latest
    # docker pull mcr.microsoft.com/powershell:ubuntu-bionic
    # docker pull mcr.microsoft.com/windows/servercore:ltsc2019
    # docker pull mcr.microsoft.com/windows/nanoserver:1903
    # docker pull mcr.microsoft.com/mssql/server:2017-latest
    # docker pull mysql:5.7
    # docker pull postgres:latest
    # docker pull redis:latest
    # docker pull wordpress:latest
    # docker pull vault:latest
}



Task "docker:local:update" {

    $config = Read-PublishFile
    $packages = $config.Packages.Name

    $pwsh = '& { rm -Recurse -Force /modules -EA SilentlyContinue; New-Item /modules -ItemType Directory }'

    docker exec -i gz-nano-pwsh powershell.exe -Command $pwsh 
    docker exec -i gz-ubuntu-pwsh /bin/bash -c 'rm -rf /modules && mkdir /modules'
    
    docker stop gz-nano-pwsh 
    docker stop gz-ubuntu-pwsh 

    foreach($package in $packages) {
        docker cp "$package/." gz-nano-pwsh:/modules/$package
        docker cp "$package/." gz-ubuntu-pwsh:/modules/$package
    }

    docker start gz-nano-pwsh
    docker start gz-ubuntu-pwsh  
}

Task "test:local:docker:build" {

    $dockerFiles = Get-Item "$PsScriptRoot/docker/DockerFile.*"
    foreach($file in $dockerFiles) {
        docker build -f $file.FullName $PsScriptRoot/docker 
    }
}


function Read-PublishFile() {

    Import-Module "$PsScriptRoot/Gz-Yaml/Gz-Yaml.psd1" -Force

    return Get-Content "$PsScriptRoot/publish.yml" -Raw | ConvertFrom-Yaml 
}