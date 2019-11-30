# escape=`

# Use the latest Windows Server Core image with .NET Framework 4.8.
FROM mcr.microsoft.com/dotnet/framework/sdk:4.8-windowsservercore-ltsc2019


RUN Install-PackageProvider -Name NuGet -RequiredVersion 2.8.5.201 -Force; `
    Install-Module Gz-ChocolateySync -Force; `
    New-Item C:/projects -ItemType Directory; `
    New-Item C:/apps -ItemType Directory; 


# Restore the default Windows shell for correct batch processing.
SHELL ["cmd", "/S", "/C"]


# Download the Build Tools bootstrapper.
COPY vs_BuildTools.exe C:\apps\vs_buildtools.exe

# Install Build Tools excluding workloads and components with known issues.
RUN C:\apps\vs_buildtools.exe --quiet --wait --norestart --nocache `
    --add Microsoft.VisualStudio.Workload.NetCoreBuildTools `
    --add Microsoft.VisualStudio.Workload.WebBuildTools `
 || IF "%ERRORLEVEL%"=="3010" EXIT 0


VOLUME C:/projects

# Start developer command prompt with any other commands specified.
#ENTRYPOINT C:\Vs\Common7\Tools\VsDevCmd.bat &&

# Default to PowerShell if no other command specified.
CMD ["powershell.exe", "-NoLogo", "-ExecutionPolicy", "Bypass"]