
function New-GzModuleManifest() {
    #TODO: Document parameters
    <#
    .SYNOPSIS
        Creates a new module manifest based on a template.

    .DESCRIPTION
        Creates a new module manifest based on a hashtable template, using
        this command's parameters to override the template values.

    .EXAMPLE
        PS C:\> New-GzModuleManifiest -Path "./Gz-NewModule.psd1" -Template $template
    
    .INPUTS
        Inputs (if any)
    .OUTPUTS
        Output (if any)
    .NOTES
        General notes
    #>
    Param(
        [String] $Path,

        [hashtable] $Template,

        [String] $Author,

        [String] $CompanyName,

        [String] $RootModule,

        [String] $Copyright,

        [String] $Description,

        [String[]] $Tags,

        [String] $ProjectUri,

        [String] $IconUri,

        [String] $LicenseUri,

        [String] $ReleaseNotes,

        [String] $PowerShellHostName,

        [String] $DotNetFrameworkVersion,

        [String] $ClrVersion,
       
        [String[]] $RequiredModules = $null,

        [String[]] $RequiredAssemblies = $null,

        [String[]] $ScriptsToProcess = $null,

        [String[]] $TypesToProcess = $null,

        [String[]] $FormatsToProcess = $null,

        [String[]] $NestedModules = $null,

        [String[]] $FunctionsToExport = $null,

        [String[]] $CmdletsToExport = $null,

        [string[]] $VariablesToExport = $null,

        [String[]] $AliasesToExport = $null
    )



    if(!$Template) {
        $Template  = @{}
        if(Test-Path "$HOME/.gz/modules/modulemanifest.json") {
            $data = (Get-Content "$HOME/.gz/modules/modulemanifest.json" -Raw) | ConvertFrom-Json 
            $hash = @{};

            $data | Get-Member -MemberType NoteProperty | Foreach-Object {
                $n = $_.Name 
                $v = $data.$n 
                $hash.Add($n, $v)
            }

            $Template = $hash 
        }
    }

    $argz = @{}
    foreach($key in $Template.Keys) {
        $argz[$key] = $Template[$key]
    }

    $params = (Get-Command New-GzModuleManifest).Parameters
    foreach($key in $params.Keys) {
       
        $pt = $params[$key].ParameterType
        if($pt -eq [String]) {
            $value = (Get-Variable -NAme $key -Scope 0).Value 
            if(![string]::IsNullOrWhitespace($value)) {
                $argz[$key] = $value
            }
        }

        if($pt -eq [String[]]) {
            $value = (Get-Variable -NAme $key -Scope 0).Value 
            if($value -and $value.Length -gt 0) {
                $argz[$key] = $value
            }
        }
    }
    return New-ModuleManifest @argz
}