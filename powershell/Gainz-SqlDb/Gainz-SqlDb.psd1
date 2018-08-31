#
# Module manifest for module 'Fmg-Sql'
#
# Generated by: mhern
#
# Generated on: 5/13/2017
#

@{

# Script module or binary module file associated with this manifest.
RootModule = 'Gainz-SqlDb.psm1'

# Version number of this module.
ModuleVersion = '0.1.0'

# Supported PSEditions
# CompatiblePSEditions = @()

# ID used to uniquely identify this module
GUID = '5961e800-31fd-4d2d-8568-b92d506372b9'

# Author of this module
Author = 'Nerdy Mishka, Michael Herndon'

# Company or vendor of this module
CompanyName = 'Nerdy Mishka'

# Copyright statement for this module
Copyright = '(c) 2018 Nerdy Mishka, Michael Herndon. All rights reserved.'

# Description of the functionality provided by this module
Description = "
Gainz: Sql Db

Database agnostic Powershell functions over ADO.NET.

The primary functions are Invoke-DbCmd, Read-DbData, and Write-DbData. If
a connection or connectionString is not provided to the functions, the functions
will attempt to use the default connection set by Set-DbConnectionString.

Any function that has a -Do parameter, can be passed a script block that will
have the `$_ context variable set with either the connection or command object.
"

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
    'Set-DbConnectionString',
    'Get-DbConnectionString',
    'New-DbProviderFactory',
    'Set-DbProviderFactory',
    'Get-DbProviderFactory',
    'Get-DbParameterPrefix',
    'Set-DbParameterPrefix',
    'New-DbConnection',
    'New-DbCommand',
    'Read-DbData',
    'Write-DbData',
    'Invoke-DbCmd'
)

# Cmdlets to export from this module, for best performance, do not use wildcards and do not delete the entry, use an empty array if there are no cmdlets to export.
CmdletsToExport = @(
    
)

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
        Tags = @('SQL', 'ADO.NET', 'Database', 'SqlClient', 'Gainz', "Gainz-SqlDb")

        # A URL to the license for this module.
        LicenseUri = 'https://www.apache.org/licenses/LICENSE-2.0'

        # A URL to the main website for this project.
        ProjectUri = 'https://gitlab.com/nerdymishka/gainz/tree/master/powershell/Gainz-SqlDb'

        # A URL to an icon representing this module.
        # TODO: gnome icon
        IconUri = 'https://nerdymishka.com/wp-content/themes/bad-mishka/images/logo.png'

        ReleaseNotes = '
0.1.0 - Early Release
        
- Read-DbData only supports one return set.
- Tests currently target SqlServer '

    } # End of PSData hashtable

} # End of PrivateData hashtable

# HelpInfo URI of this module
# HelpInfoURI = ''

# Default prefix for commands exported from this module. Override the default prefix using Import-Module -Prefix.
# DefaultCommandPrefix = ''

}

