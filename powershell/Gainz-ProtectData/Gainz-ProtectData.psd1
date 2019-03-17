#
# Module manifest for module 'Fmg-ProtectData'
#
# Generated by: MichaelHerndon
#
# Generated on: 8/5/2018
#

@{

# Script module or binary module file associated with this manifest.
RootModule = '.\Gainz-ProtectData.psm1'

# Version number of this module.
ModuleVersion = '0.1.2'

# Supported PSEditions
# CompatiblePSEditions = @()

# ID used to uniquely identify this module
GUID = '20eb8659-b295-4799-89eb-d1fa631da9fc'

# Author of this module
Author = 'Nerdy Mishka, Michael Herndon'

# Company or vendor of this module
CompanyName = 'Nerdy Mishka'

# Copyright statement for this module
Copyright = '(c) 2018 Nerdy Mishka. All rights reserved.'

# Description of the functionality provided by this module
Description = '
Gainz: Protect Data Module

Deprecated: Renaming to Gz-ProtectData

ProtectData provides functions/cmdlets that will encrypt and decrypt string or
binary data with a SecureString password or a private key made of an array of 
ytes.

By default, the encrypted value will also contain a computed hash of the
data to verify integrity.
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
FunctionsToExport = @(
    'Protect-Blob',
    "Protect-String",
    'Get-ProtectOptions',
    "Unprotect-Blob",
    "Unprotect-String",
    "ConvertTo-UnprotectedBytes"
)

# Cmdlets to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no cmdlets to export.
CmdletsToExport = @()

# Variables to export from this module
VariablesToExport = @()

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
        Tags = @("Protect", "Protect-String", "Protect-Blob", "Data", "DevOps", "Gz", "Gainz", "NerdyMishka")

        LicenseUri = 'https://www.apache.org/licenses/LICENSE-2.0'
 
        # A URL to the main website for this project.
        ProjectUri =  'https://gitlab.com/nerdymishka/gainz/tree/master/powershell/Gz-ProtectData'
 
        # A URL to an icon representing this module.
        IconUri = 'https://gitlab.com/nerdymishka/NerdMishka/raw/dev/resources/images/logo.png'
 
 
        # ReleaseNotes of this module
        ReleaseNotes = '
0.1.2 - Renaming to Gz-ProtectData
0.1.0 - Early Release

The module is not documented yet. Blob and String encryption works with a 
password or a private key made of bytes.  
         '

    } # End of PSData hashtable

} # End of PrivateData hashtable

# HelpInfo URI of this module
# HelpInfoURI = ''

# Default prefix for commands exported from this module. Override the default prefix using Import-Module -Prefix.
# DefaultCommandPrefix = ''

}

