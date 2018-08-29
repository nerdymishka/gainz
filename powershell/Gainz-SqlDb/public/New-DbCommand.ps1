
function New-DbCommand() {
<#
    .SYNOPSIS
    Creates a new SQL command object

    .DESCRIPTION
    Creates a new SQL command, sets the command type to text by default, requires the
    SQL statement, and allows users to specify parameters. 

    If the `Do` parameter is not specified, the cmd object is returned, otherwise
    the script block expects the cmd to be executed within the script block and is 
    disposed

    .PARAMETER Connection
    (Optional) The connection object used to create the command and to execute the query.

    .PARAMETER Transaction
    (Optional) The transaction object used to be applied to the command. 
    

    .PARAMETER Query
    The SQL statement that will be excuted by this command object.

    .PARAMETER Parameters
    (Optional). The parameters object can be a PsObject, Hashtable, or an Array. The keys for the hashtable and the
    property names for the PsObject are prefixed with the parameter prefix and used as SQL Parameter names.
    
    For an Array, the prefix is prepend to the index for the SQL Parameter name.

    .PARAMETER ParameterPrefix
    (Optional) Defaults to `@`. The symbol used to notate a parameter in the SQL statement.

    .PARAMETER CommandType
    (Optional) Defaults to 'Text'. Specify the command type for the command.

    .PARAMETER Do
    (Optional) If specified,  two variables are bound to the
    script block, `$_` and `$Command` which can be used within the script block. The
    command is disposed one the script block is executed. 

    .EXAMPLE
     PS C:\> $Connection | New-DbCommand "SELECT * FROM [People]" -Do { 
     PS C:\>       $dr = $_.ExecuteReader(); 
     PS C:\>       While($dr.Read()) { 
     PS C:\>            Write-Host ($dr.GetValue(0)) 
     PS C:\>       } 
     PS C:\> }
 
    .EXAMPLE
    $cmd = $Connection | New-DbCommand  "Select @Value AS Value" -Parameters @{"Value" = 11}

#>
    [CmdletBinding()]
    Param(
        [Parameter(ValueFromPipeline =$True)]
        [System.Data.IDbConnection] $Connection,
        
        [Parameter(ValueFromPipeline =$True)]
        
        [System.Data.IDbTransaction] $Transaction,
        [Parameter(Mandatory = $true, Position = 0)]
        
        [string] $Query,
        
        [Object] $Parameters,
        
        [string] $ParameterPrefix,
        
        [System.Data.CommandType] $CommandType = "Text",
        
        [ScriptBlock] $Do 
    )

    if($Transaction -eq $null -and $Connection -eq $Null) {
        throw [System.ArgumentNullException] ("Either a Transaction or Connection MUST be specified ")
    }

    if($Transaction -ne $Null) {
        $Connection = $Transaction.Connection;
    }

    $Cmd = $Connection.CreateCommand();

    if($Transaction -ne $Null) {
        $Cmd.Transaction = $Transaction;
    }

    $Cmd.CommandType = $CommandType
    if(![string]::IsNullOrWhiteSpace($Query)) {
        $Cmd.CommandText = $Query;
    }

  

    if($Parameters) {
        $Cmd | Add-DbParameters -Parameters $Parameters -ParameterPrefix $ParameterPrefix
    }

    $Command = $Cmd 
    Set-Variable -Name "_" -Value $cmd
    if($Do) {
        $vars = @(
            (Get-Variable -Name "Command" -Scope 0),
            (Get-Variable -Name "Cmd" -Scope 0) 
            (Get-Variable -Name "_" )
        )
        try {
            $Do.InvokeWithContext(@{}, $vars)
        } finally {
            $cmd.Dispose()
        }
        return;
    }

    return $cmd
}
