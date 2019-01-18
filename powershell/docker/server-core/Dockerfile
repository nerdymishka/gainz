FROM  mcr.microsoft.com/windows/servercore:1809

SHELL ["powershell.exe", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

RUN Write-Host "Hello World"
RUN New-Item -ItemType Directory "C:/modules" 

COPY ./modules /modules/

RUN Get-Item "c:/modules/Gainz-PasswordGenerator"