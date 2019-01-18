#
# Module manifest for module 'Gz-WindowsTelemetry'
#
# Generated by: MichaelHerndon
#
# Generated on: 1/18/2019
#

@{

# Script module or binary module file associated with this manifest.
RootModule = '.\Gz-WindowsTelemetry.psm1'

# Version number of this module.
ModuleVersion = '0.1.0'

# Supported PSEditions
# CompatiblePSEditions = @()

# ID used to uniquely identify this module
GUID = '3b7d5531-a9c8-4103-a988-e53570f97109'

# Author of this module
Author = 'Nerdy Mishka, Michael Herndon'

# Company or vendor of this module
CompanyName = 'Nerdy Mishka'

# Copyright statement for this module
Copyright = '(c) 2019 Nerdy Mishka, Michael Herndon. All rights reserved.'

# Description of the functionality provided by this module
Description = '
Module to pull telemetry data from windows for corporate environments. 

This module exists because Intune is lacking with the MDM Windows 10 integration.

- installed win 32 apps
- installed appx packages
- enabled windows features
- local users
- local groups
- administrators
- powershell version and modules
- cloud join users
- bit locker status
- volumes
- chrome extensions
- user files
'

# Minimum version of the Windows PowerShell engine required by this module
# PowerShellVersion = ''

# Name of the Windows PowerShell host required by this module
# PowerShellHostName = ''

# Minimum version of the Windows PowerShell host required by this module
# PowerShellHostVersion = ''

# Minimum version of Microsoft .NET Framework required by this module. This prerequisite is valid for the PowerShell Desktop edition only.
# DotNetFrameworkVersion = ''

# Minimum version of the common language runtime (CLR) required by this module. This prerequisite is valid for the PowerShell Desktop edition only.
# CLRVersion = ''

# Processor architecture (None, X86, Amd64) required by this module
# ProcessorArchitecture = ''

# Modules that must be imported into the global environment prior to importing this module
# RequiredModules = @()

# Assemblies that must be loaded prior to importing this module
# RequiredAssemblies = @()

# Script files (.ps1) that are run in the caller's environment prior to importing this module.
# ScriptsToProcess = @()

# Type files (.ps1xml) to be loaded when importing this module
# TypesToProcess = @()

# Format files (.ps1xml) to be loaded when importing this module
# FormatsToProcess = @()

# Modules to import as nested modules of the module specified in RootModule/ModuleToProcess
# NestedModules = @()

# Functions to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no functions to export.
FunctionsToExport =  @(
    'Get-GzWinAutopilotInfo',
    'Get-GzWinCloudJoinUser',
    'Get-GzWinLocalGroupMember',
    'Invoke-GzWinTelemetry',
    'Merge-GzWinTelemetryObject',
    'Read-GzPowershellModule',
    'Read-GzWin32App',
    'Read-GzWinAdministratorMember',
    'Read-GzWinAppXPackage',
    'Read-GzWinBitLockerStatus',
    'Read-GzWinChromeExtension',
    'Read-GzWinEnabledFeature',
    'Read-GzWinLocalUser',
    'Read-GzWinService',
    'Read-GzWinUserFile',
    'Read-GzWinVolume',
    'Register-GzRegistryUserHive'
)

# Cmdlets to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no cmdlets to export.
CmdletsToExport = @()

# Variables to export from this module
VariablesToExport = ''

# Aliases to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no aliases to export.
AliasesToExport = @()

# DSC resources to export from this module
# DscResourcesToExport = @()

# List of all modules packaged with this module
# ModuleList = @()

# List of all files packaged with this module
# FileList = @()

# Private data to pass to the module specified in RootModule/ModuleToProcess. This may also contain a PSData hashtable with additional module metadata used by PowerShell.
PrivateData = @{

    PSData = @{

        # Tags applied to this module. These help with module discovery in online galleries.
        Tags = @('VisualStudio', "VS", "MsBuild", "Test", "VsTest", "DevOps", "Gainz", "NerdyMishka")

        LicenseUri = 'https://www.apache.org/licenses/LICENSE-2.0'

        # A URL to the main website for this project.
        ProjectUri =  'https://gitlab.com/nerdymishka/gainz/tree/master/powershell/Gz-WindowsTelemetry'

        # A URL to an icon representing this module.
        IconUri = 'https://gitlab.com/nerdymishka/NerdyMishka/raw/dev/resources/images/logo.png'


        # ReleaseNotes of this module
        ReleaseNotes = '
- 0.1.0 initial release. The goal is to get to a point where the json files are treated as records that change over time.
  Currently the output from Invoke-GzWinTelemetry should be saved to a new file with each invoke. The result is PsCustomObject
  which can exported to JSON, YAML, XML, and if need be, csv if you generate csv files by leaf.  
  
  The next version major release 0.2.0 will focus on reporting. 
'

    } # End of PSData hashtable

} # End of PrivateData hashtable

# HelpInfo URI of this module
# HelpInfoURI = ''

# Default prefix for commands exported from this module. Override the default prefix using Import-Module -Prefix.
# DefaultCommandPrefix = ''

}

