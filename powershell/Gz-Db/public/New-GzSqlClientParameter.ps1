

function New-GzSqlClientParameter() {
<#
    .SYNOPSIS
    Creates a new System.Data.SqlClient.SqlParameter object.

    .DESCRIPTION
    By default, the .NET framework has an implementation for SqlClient and 
    thus this cmdlet exists to help create new SqlParameter objects.

    .PARAMETER ParameterName
    The name of the parameter. The `Name` Parameter is an alias for this one.

    .PARAMETER DbType
    The `System.Data.SqlClient.SqlDbType` type for this SQL parameter.

    .PARAMETER Value
    The value that will be bound to this parameter.

    .PARAMETER Size
    (Optional). Sets the maximum size in bytes for this parameter.

    .PARAMETER Scale
    (Optional). Sets the number of decimal places to which the `Value` is resolved

    .PARAMETER Precision
    (Optional). Sets the maximum number of digits used to represent the Value.

    .EXAMPLE
     $parameter = New-GzSqlClientParameter "FirstName" "NVarChar" "Nerdy" -Limit 255

     .LINK
     https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlparameter(v=vs.110).aspx

     .LINK
     https://msdn.microsoft.com/en-us/library/system.data.sqlclient.sqlparameter.sqldbtype(v=vs.110).aspx
#>
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 1)]
        [Alias("Name")]
        [string] $ParameterName,
        [Parameter(Mandatory = $true, Position = 2)]
        [System.Data.SqlClient.SqlDbType] $DbType,
        [Parameter(Mandatory = $true, Position = 3)]
        [object] $Value,
        [Nullable[int]] $Size = $null,
        [Nullable[byte]] $Precision = $null,
        [Nullable[int]] $Scale = $null 
    )
    [System.Data.SqlClient.SqlDbType]::
    $parameter = New-Object System.Data.SqlClient.SqlParameter 
    $parameter.ParameterName = "@$ParameterName";
    $parameter.SqlDbType = $DbType;
    $parameter.Value = $Value;

    if($Size.HasValue) {
        $parameter.Size = $Size.Value;
    }

    if($Precision.HasValue) {
        $parameter.Size = $Precision.Value;
    }

    if($Scale.HasValue) {
        $parameter.Size = $Scale.Value;
    }

    return $parameter;
}