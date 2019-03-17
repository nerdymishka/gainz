# Gz-VisualStudio

A powershell module that facilitates leveraging msbuild
and the Visual Studio test console with powershell cmdlets.

The most useful commands are:

- **Invoke-GzMsBuild** -- invokes a build using ms build
- **Invoke-GzVisualStudioBuild** -- invokes a build using ms build installed by visual studio.
- **Invoke-GzVisualStudioTestConsole** -- invokes the visual studio test console to test assemblies.
- **Read-GzVisualStudioSolution** -- read solution files and provides metadata for projects in the solution such as project path, project types, etc.
- **Add-GzVisualStudioAlias** -- addes the following aliases
  - Add-VsVersionAlias => Add-GzVisualStudioVersionAlias
  - Clear-VsVersionCache => Clear-GzVisualStudioVersionCache
  - Get-MsBuildPath => Get-GzMsBuildPath
  - Get-VsBuildToolsPath => Get-GzVisualStudioBuildToolsPath
  - Get-VsPath => Get-GzVisualStudioPath
  - Get-VsTestPath => Get-GzVisualStudioTestConsolePath
  - Get-VsVersion => Get-GzVisualStudioVersion
  - Invoke-MsBuild => Invoke-GzMsBuild
  - Invoke-VsBuild => Invoke-GzVisualStudioBuild
  - Invoke-VsTest => Invoke-GzVisualStudioTestConsole
  - Read-VsSolution => Read-GzVisualStudioSolution