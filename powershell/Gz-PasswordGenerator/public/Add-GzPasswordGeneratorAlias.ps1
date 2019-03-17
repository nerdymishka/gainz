$gzAliases = @{
    "New-Password" = "New-GzPassword"
};

function Remove-GzPasswordGeneratorAlias() {
    <#
    .SYNOPSIS
        Removes the aliases that map to Gz prefixed commands for
        the Gz-PasswordGenerator module.

    .DESCRIPTION
        Removes the aliases that map to Gz prefixed commands. 
        For example, removes the alias New-Password that points
        to New-GzPassword. 
    .EXAMPLE
        PS C:\> Remove-GzPasswordGeneratorAlias
    .INPUTS
        None
    .OUTPUTS
        None
    .NOTES
        Aliases are to remove the Gz Prefix
    #>

    foreach($key in $gzAliases.Keys) {
        if($null -ne (Get-Alias $key -EA SilentlyContinue)) {
            Remove-Item alias:\$key 
        }
    }
}

function Add-GzPasswordGeneratorAlias() {
 <#
    .SYNOPSIS
        Adds the aliases that map to Gz prefixed commands for
        the Gz-PasswordGenerator module.

    .DESCRIPTION
        Adds the aliases that map to Gz prefixed commands. 
        For example, adds the alias New-Password that points
        to New-GzPassword. 
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