Task "docker:pull" {
    docker pull ubuntu:latest
    docker pull mcr.microsoft.com/powershell:ubuntu-bionic
    docker pull mcr.microsoft.com/windows/servercore:ltsc2019
    docker pull mcr.microsoft.com/windows/nanoserver:1903
    docker pull mcr.microsoft.com/mssql/server:2017-latest
    docker pull mysql:5.7
    docker pull postgres:latest
    docker pull redis:latest
    docker pull wordpress:latest
    docker pull vault:latest
}



Task "test:local:docker:prep" {

    $config = Read-PublishFile
    $packages = $config.Packages.Name

    foreach($package in $packages) {
        $dest = "$PsScriptRoot/docker/modules/$package"
        if(Test-Path $dest) {
            Remove-Item "$dest" -Force -Recurse  | Write-Debug 
        }

        Copy-Item "$psScriptRoot/$package" $dest -Recurse -Force | Write-Debug 
    }
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