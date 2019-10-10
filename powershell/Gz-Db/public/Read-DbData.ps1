function Read-DbData() {
<#
    .SYNOPSIS
    Reads data from a SQL query

    .DESCRIPTION
    Creates a cmd object, sets the query and parameters, and invokes ExecuteReader on the 
    command. This cmdlet only supports single result sets like selecting one or more
    rows from a table. 

    .PARAMETER ConnectionString
    (Optional) The connection string is used to create and open the connection.

    .PARAMETER Connection
    (Optional) The connection object used to create the command and to execute the query. 
    If the connection is not supplied, one is created, opened, and closed within in the cmdlet.


    .PARAMETER Query
    The SQL statement that will be excuted by this command object.

    .PARAMETER Parameters
    (Optional). The parameters object can be a PsObject, Hashtable, or an Array. The keys for the hashtable and the
    property names for the PsObject are prefixed with the parameter prefix and used as Sql Parameter names.
    
    For an Array, the prefix is prepend to the index for the Sql Parameter name.

    .PARAMETER ParameterPrefix
    (Optional) Defaults to '@'. The symbol used to notate a parameter in the SQL statement.

    .EXAMPLE
     $data = $Connection | Read-DbData "SELECT * FROM [table]" 

    .EXAMPLE
     $results = Read-DbData "SELECT FirstName, LastName, Age as [Years] from [People]" -ConnectionString "Data Source=(LocalGzDb)\MSSQLLocalGzDb;Integrated Security=True"
#>
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 1)]
        [String] $Query,
        
        [Object] $Parameters,
        
        [Alias("c")]
        [string] $ConnectionString,

        [Alias("cn")]
        [string] $ConnectionStringName,

        [Alias("pn")]
        [String] $ProviderName = "Default",
        
        [Parameter(ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [System.Data.IDbConnection] $Connection,
        
        [String] $ParameterPrefix = $null 
    )

    # TODO: handle parameters better in the BEGIN block
    # TODO: wrap execution inside the PROCESS block
    # TODO: wrap up disposal insicd the END block

    
    if(!$Connection -and [string]::IsNullOrWhiteSpace($ConnectionString)) {
        if(![string]::IsNullOrWhiteSpace($ConnectionStringName)) {
            $ConnectionString = Get-DbConnectionString -Name $Name 
            if([String]::IsNullOrWhiteSpace($ConnectionString)) {
                throw "Could not find connection string for $Name"
            }
        } else {
            $ConnectionString = Get-DbConnectionString
        }

        if([string]::IsNullOrWhiteSpace($ConnectionString)) {
            $msg =  "The ConnectionString parameter or global connection string MUST "
            $msg += "be set before communicating with SQL SERVER." 
            throw [System.ArgumentException] $msg
        }
    } 

    $dispose = $false 
    $open = $false 
    if(!$Connection) {
        $dispose = $true;
        $factory = Get-DbProviderFactory $ProviderName
        $obj  = $factory.CreateConnection()
        
        $Connection = [System.Data.IDbConnection]$obj
    }

    if(!$Connection.ConnectionString) {
        $Connection.ConnectionString = $ConnectionString
    }
    

    $cmd = $null;
    $dr = $null;
    try {
        if($Connection.State -ne "Open") {
            $Connection.Open()
            $open = $true;
        }

        $cmd = $Connection | New-DbCommand $Query -Parameters $Parameters -ParameterPrefix $ParameterPrefix
        $dr = $cmd.ExecuteReader()
        
        $results = @() 
        # TODO: handle multiple result sets. 
        while($dr.Read())
        {
            $item = New-Object PSObject 
            for($i = 0; $i -lt $dr.FieldCount; $i++) {
                $name = ($dr.GetName($i))
                if(!$name) {
                    $name = "column $i"
                }
                $item | Add-Member -MemberType NoteProperty -Name $name -Value ($dr.GetValue($i))
            }
            $results += $item;
        }
        
        
        return $results
    } finally {
        if($dr) {
            $dr.Dispose();
        }
        if($cmd) {
            $cmd.Dispose();
        }
        if($open) {
            $Connection.Close();
        }
        if($dispose) {
            $Connection.Dispose()
        }
    }
}