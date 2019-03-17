$gzAliases = @{
    "Add-VsVersionAlias" = "Add-GzVisualStudioVersionAlias"
    "Clear-VsVersionCache" = "Clear-GzVisualStudioVersionCache"
    "Get-MsBuildPath" = "Get-GzMsBuildPath"
    "Get-VsBuildToolsPath" = "Get-GzVisualStudioBuildToolsPath"
    "Get-VsPath" = "Get-GzVisualStudioPath"
    "Get-VsTestPath" = "Get-GzVisualStudioTestConsolePath"
    "Get-VsVersion" = "Get-GzVisualStudioVersion"
    "Invoke-MsBuild" = "Invoke-GzMsBuild"
    "Invoke-VsBuild" = "Invoke-GzVisualStudioBuild"
    "Invoke-VsTest" = "Invoke-GzVisualStudioTestConsole"
    "Read-VsSolution" = "Read-GzVisualStudioSolution"
};

function Remove-GzVisualStudioAlias() {
    <#
    .SYNOPSIS
        Removes the aliases that map to Gz prefixed commands for
        the Gz-PasswordGenerator module.

    .DESCRIPTION
        Removes the aliases that map to Gz prefixed commands. 
        For example, removes the alias Invoke-VsBuild that points
        to Invoke-GzVisualStudioBuild. 
    .EXAMPLE
        PS C:\> Remove-GzPasswordGeneratorAlias
    .INPUTS
        None
    .OUTPUTS
        None
    .NOTES
        - "Add-VsVersionAlias" => "Add-GzVisualStudioVersionAlias"
        - "Clear-VsVersionCache" => "Clear-GzVisualStudioVersionCache"
        - "Get-MsBuildPath" => "Get-GzMsBuildPath"
        - "Get-VsBuildToolsPath" => "Get-GzVisualStudioBuildToolsPath"
        - "Get-VsPath" => "Get-GzVisualStudioPath"
        - "Get-VsTestPath" => "Get-GzVisualStudioTestConsolePath"
        - "Get-VsVersion" => "Get-GzVisualStudioVersion"
        - "Invoke-MsBuild" => "Invoke-GzMsBuild"
        - "Invoke-VsBuild" => "Invoke-GzVisualStudioBuild"
        - "Invoke-VsTest" => "Invoke-GzVisualStudioTestConsole"
        - "Read-VsSolution" => "Read-GzVisualStudioSolution"
    #>

    foreach($key in $gzAliases.Keys) {
        if($null -ne (Get-Alias $key -EA SilentlyContinue)) {
            Remove-Item alias:\$key 
        }
    }
}

function Add-GzVisualStudioAlias() {
 <#
    .SYNOPSIS
        Adds the aliases that map to Gz prefixed commands for
        the Gz-VisualStudio module.

    .DESCRIPTION
        Adds the aliases that map to Gz prefixed commands. 
        For example, adds the alias Invoke-VsBuild that points
        to Invoke-GzVisualStudioBuild. 
    .EXAMPLE
        PS C:\> Add-GzPasswordGeneratorAlias
    .INPUTS
        Inputs (if any)
    .OUTPUTS
        Output (if any)
    .NOTES
        Aliases are to remove the Gz Prefix
    #>
    foreach($key in $gzAliases.Keys) {
        if($null -eq (Get-Alias $key -EA SilentlyContinue)) {
            Set-Alias -Name ($key) -Value $gzAliases[$key] -Scope Global 
        }
    }
}