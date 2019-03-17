$gzAliases = @{
    "New-DbConnection" = "New-GzDbConnection"
    "Invoke-DbCommand" = "Invoke-GzDbCommand"
    "Read-DbData" = "Read-GzDbData"
    "Write-DbData" = "Write-GzDbData"
    "Get-DbConnectionString" = "Get-GzDbConnectionString"
    "Set-DbConnectionString" = "Set-GzDbConnectionString"
};

function Remove-GzDbAlias() {
    <#
    .SYNOPSIS
        Removes the aliases that map to Gz prefixed commands for
        the Gz-Db module.

    .DESCRIPTION
        Removes the aliases that map to Gz prefixed commands. 
        For example, removes the alias New-DbConnection that points
        to New-GzDbConnection. 
    .EXAMPLE
        PS C:\> Remove-GzDbAlias
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

function Add-GzDbAlias() {
 <#
    .SYNOPSIS
        Adds the aliases that map to Gz prefixed commands for
        the Gz-Db module.

    .DESCRIPTION
        Adds the aliases that map to Gz prefixed commands. 
        For example, adds the alias New-DbConnection that points
        to New-GzDbConnection. 
    .EXAMPLE
        PS C:\> Add-GzDbAlias
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