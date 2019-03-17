function Invoke-GzDbCommand() {
<#
    .SYNOPSIS
    Creates and invokes a new sql command using ExecuteNonQuery

    .DESCRIPTION
    Creates and invokes a new sql command using ExecuteNonQuery

    .PARAMETER ConnectionString
    (Optional) The connection string is used to create and open the connection.

    .PARAMETER Connection
    (Optional) The connection object used to create the command and to execute the query. 
    If the connection is not supplied, one is created, opened, and closed within in the cmdlet.

    .PARAMETER Transaction
    (Optional) The transaction object used to be applied to the command.  If the `UseTransaction`
    switch is present a transaction is created and committed or rolledback within in the cmdlet.
    
    .PARAMETER UseTransaction
    (Optional) The cmdlet will create a transaction from the connection if one
    is not supplied, for this query.

    .PARAMETER Query
    The SQL statement that will be excuted by this command object.

    .PARAMETER Parameters
    (Optional). The parameters object can be a PsObject, Hashtable, or an Array. The keys for the hashtable and the
    property names for the PsObject are prefixed with the parameter prefix and used as Sql Parameter names.
    
    For an Array, the prefix is prepend to the index for the Sql Parameter name.

    .PARAMETER ParameterPrefix
    (Optional) Defaults to `@`. The symbol used to notate a parameter in the SQL statement.

    .EXAMPLE
    PS:/> $Connection | Invoke-GzDbCommand "DROP DATABASE FMG" 

    .EXAMPLE
    Invoke-GzDbCommand "DROP DATABASE FMG" -ConnectionString "Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True"
#>
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $true, Position = 0)]
        [String] $Query,
        
        [Object] $Parameters,
        
        [Alias("c")]
        [string] $ConnectionString,

        [Alias("cn")]
        [string] $ConnectionStringName,

        [Alias("pn")]
        [String] $ProviderName = "Default",
        
        [Parameter(ValueFromPipeline = $True)]
        [System.Data.IDbConnection] $Connection,
        
        [Parameter(ValueFromPipeline = $True)]
        [System.Data.IDbTransaction] $Transaction,
        
        [Switch] $UseTransaction,
        
        [String] $ParameterPrefix = $null 
    )

    # TODO: handle parameters better in the BEGIN block
    # TODO: wrap execution inside the PROCESS block
    # TODO: wrap up disposal insicd the END block
    
    if(!$Connection -and !$Transaction -and [string]::IsNullOrWhiteSpace($ConnectionString)) {
        if(![string]::IsNullOrWhiteSpace($ConnectionStringName)) {
            $ConnectionString = Get-GzDbConnectionString -Name $Name 
            if([String]::IsNullOrWhiteSpace($ConnectionString)) {
                throw "Could not find connection string for $Name"
            }
        } else {
            $ConnectionString = Get-GzDbConnectionString
        }
        if([string]::IsNullOrWhiteSpace($ConnectionString)) {
            $msg =  "The ConnectionString parameter or global connection string MUST "
            $msg += "be set before communicating with SQL SERVER." 
            throw [System.ArgumentException] $msg
        }
    } 


    $disposeTransaction = $false 
    $disposeConnection = $false 
    $closeConnection = $false 
   

    if($Transaction -ne $null) {
        
        if(!$Connection) {
            $Connection = $Transaction.Connection;
        }
      
    } elseif($UseTransaction.ToBool()) {
      
        $disposeTransaction = $true;
        $closeConnection = $true 
 

        if(!$Connection) {
            $factory = Get-GzDbProviderFactory $ProviderName
            $Connection = $factory.CreateConnection()
            $Connection.ConnectionString = $ConnectionString
            $Connection.Open()
            $closeConnection = $true 
            $disposeConnection = $true 

        } else {
            if($Connection.State -ne "Open") {
                if(!$Connection.ConnectionString) {
                    $Connection.ConnectionString = $ConnectionString;
                }
                $Connection.Open()
                $closeConnection = $true 
            }
        }

        $Transaction = $Connection.BeginTransaction()
    } else {
        if(!$Connection) {
            $factory = Get-GzDbProviderFactory $ProviderName
            $Connection = $factory.CreateConnection()
            $Connection.ConnectionString = $ConnectionString
            $Connection.Open()
            $closeConnection = $true 
            $disposeConnection = $true 
        } else {
            if($Connection.State -ne "Open") {
                if(!$Connection.ConnectionString) {
                    $Connection.ConnectionString = $ConnectionString;
                }
                $Connection.Open()
                $closeConnection = $true 
            }
        }
    }
    

    $cmd = $null;
    $dr = $null;
    try {
        if($Transaction) {
           $cmd = $Transaction | New-DbCommand $Query -Parameters $Parameters -ParameterPrefix $ParameterPrefix
        } else {
            $cmd = $Connection | New-DbCommand $Query -Parameters $Parameters -ParameterPrefix $ParameterPrefix
        }
       
    
        $result =  $cmd.ExecuteNonQuery();

        # Commit only if this command controls the Transaction object.
        if($Transaction -and $disposeTransaction) {
            $Transaction.Commit()
        }

        return $result;
    } catch {
        # always rollback upon error 
        if($Transaction) {
            $Transaction.Rollback()
        }
        Throw $_.Exception 
    } finally {
       
        if($cmd) {
            $cmd.Dispose();
        }
        if($closeConnection) {
            $Connection.Close();
        }
        if($disposeTransaction) {
            $Transaction.Dispose();
        }
        if($disposeConnection) {
            $Connection.Dispose();
        }
    }
}
