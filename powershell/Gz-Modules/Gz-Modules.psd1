#
# Module manifest for module 'Gz-Modules'
#
# Generated by: MichaelHerndon
#
# Generated on: 11/10/2019
#

@{

# Script module or binary module file associated with this manifest.
RootModule = './Gz-Modules.psm1'

# Version number of this module.
ModuleVersion = '0.1.0'

# Supported PSEditions
CompatiblePSEditions = 'Core', 'Desktop'

# ID used to uniquely identify this module
GUID = 'f738555a-d38c-4a2c-9d3f-f34ecb62ba4e'

# Author of this module
Author = 'Michael Herndon'

# Company or vendor of this module
CompanyName = 'Nerdy Mishka'

# Copyright statement for this module
Copyright = '(c) 2019 Nerdy Mishka, Michael Herndon. All rights reserved.'

# Description of the functionality provided by this module
Description = '
Gz-Modules

Enhanced functions for publishing powershell modules. Publish-GzModule
can modify a package so that a module will require other powershell
module dependencies and will exclude folders and files that do not follow
the typical powershell module design e.g. public/private/bin folders.

'

# Minimum version of the Windows PowerShell engine required by this module
PowerShellVersion = '5.0'

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
    'Get-GzPublishArtifactsDirectory',
    'Publish-GzModule',
    'Register-GzArtifactsRepository',
    'Set-GzPublishArtifactsDirectory'
)

# Cmdlets to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no cmdlets to export.
CmdletsToExport = '*'

# Variables to export from this module
VariablesToExport = ''

# Aliases to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no aliases to export.
AliasesToExport = '*'

# DSC resources to export from this module
# DscResourcesToExport = @()

# List of all modules packaged with this module
# ModuleList = @()

# List of all files packaged with this module
# FileList = @()

# Private data to pass to the module specified in RootModule/ModuleToProcess. This may also contain a PSData hashtable with additional module metadata used by PowerShell.
PrivateData = @{

    PSData = @{

        Tags = @('Gz', 'Gainz', 'Publish-GzModule', 'Module')
       
        LicenseUri = 'https://www.apache.org/licenses/LICENSE-2.0'
        
        ProjectUri = 'https://gitlab.com/nerdymishka/gainz/tree/master/powershell/Gz-Modules'
 
        IconUri = 'https://gitlab.com/nerdymishka/NerdMishka/raw/dev/resources/images/logo.png'
        # ReleaseNotes of this module
        ReleaseNotes = '
0.1.0 - initial release         
'

    } # End of PSData hashtable

} # End of PrivateData hashtable

# HelpInfo URI of this module
# HelpInfoURI = ''

# Default prefix for commands exported from this module. Override the default prefix using Import-Module -Prefix.
# DefaultCommandPrefix = ''

}

