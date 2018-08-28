function Write-DbData() {
<#
    .SYNOPSIS
    Writes data to a sql store.

    .DESCRIPTION
    Creates a single command that is used multiple times with different parameter sets to 
    insert or update data into a sql store.  

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
    The parameters object is an Array of PsObject, Hashtable, or Array objects. The keys for the hashtable and the
    property names for the PsObject are prefixed with the parameter prefix and used as Sql Parameter names.
    
    For an Array, the prefix is prepend to the index for the Sql Parameter name.

    .PARAMETER ParameterPrefix
    (Optional) Defaults to '@'. The symbol used to notate a parameter in the SQL statement.

    .EXAMPLE
     $Connection | Write-DbData "INSERT INTO [People] (FirstNAme) Values (@FirstName)" -Parameters @(@{"FirstName" = "test"}) 

     .EXAMPLE
     $results = Write-DbData "INSERT INTO [People] (FirstNAme) Values (@FirstName)" -Parameters @(@{"FirstName" = "test"})  -ConnectionString "Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True"
#>
    [CmdletBinding()]
    Param(
        [Parameter(Mandatory = $True, Position= 0)]
        [string] $Query,
        [Object] $Parameters,
        [string] $ConnectionString,
        [Parameter(ValueFromPipeline = $true, ValueFromPipelineByPropertyName = $true)]
        [System.Data.IDbTransaction] $Transaction,
        [Parameter(ValueFromPipeline= $True)]
        [System.Data.IDbConnection] $Connection,
        [string] $ParameterPrefix = $null
    )

    # TODO: handle parameters better in the BEGIN block
    # TODO: wrap execution inside the PROCESS block
    # TODO: wrap up disposal insicd the END block

    if(!$Transaction -and !$Connection -and [string]::IsNullOrWhiteSpace($ConnectionString)) {
        $ConnectionString = Get-DbConnectionString
        if([string]::IsNullOrWhiteSpace($ConnectionString)) {
            $msg =  "The ConnectionString parameter or global connection string MUST "
            $msg += "be set before communicating with the Database SERVER." 
            throw [System.ArgumentException] $msg
        }
    } 


    $disposeTransaction = $false 
    $disposeConnection = $false;
    $closeConnection = $false 
    $Connection = $null 

    if($Transaction -ne $Null) {
        if($Connection -eq $Null) {
            $Connection = $Transaction.Connection
        }
    } else {       
        $disposeTransaction = $true;        
 
        $factory = Get-DbProviderFactory
        if($Connection -eq $null) {
            $Connection = $factory.CreateConnection()
            $Connection.ConnectionString = $ConnectionString
            $Connection.Open()

            $disposeConnection = $true;
            $closeConnection = $True 
        } else {
            if($Connection.State -ne "Open") {
                $closeConnection = $True;
                if(!$Connection.ConnectionString) {
                    $Connection.ConnectionString = $ConnectionString;
                }
                $Connection.Open();
            }
        }
        
        $Transaction = $Connection.BeginTransaction()
    }


    $cmd = $null;
    try {
        

        $cmd = $Connection | New-DbCommand $Query
        $cmd.Transaction = $Transaction;
        $i = 0;
        $results = @()
        if(!($Parameters -is [Array])) {
            $Parameters = @($Parameters)
        }

        foreach($parameterSet in $Parameters) {
            if($i -eq 0) {
                $cmd | Add-DbParameters -Parameters $parameterSet -ParameterPrefix $ParameterPrefix
            } else {
                $cmd | Add-DbParameters -Parameters $parameterSet -ParameterPrefix $ParameterPrefix -Update:$True
            }
            $i++;
            $result = $cmd.ExecuteScalar();
            $results += $result 
        }

        
        # in case we're grabbing an Id or value.
        

        # if this class created the transaction, commit. 
        if($disposeTransaction) {
            $Transaction.Commit()
        }

        return $results;
    } catch {
        # if there is failure Rollback
        $Transaction.Rollback();
        Throw $_.Exception
    } finally {
        
        if($cmd) {
            $cmd.Dispose()
        }
        if($closeConnection) {
            $Connection.Close()
        }
        if($disposeTransaction) {
            $Transaction.Dispose()
        }
        if($disposeConnection) {
            $Connection.Dispose()
        }
    }
}