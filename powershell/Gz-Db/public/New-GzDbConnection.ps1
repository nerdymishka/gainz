function New-GzDbConnection() {
<#
    .SYNOPSIS
    Creates a new sql connection object

    .DESCRIPTION
    This cmdlet is called in absense of a specified connection string 
    for cmdlet that require them.  

    .PARAMETER ConnectionString
    (Optional) If the `Do` parameter is specified, a connection string is not 
    optional.  The connection string is used to create and open the connection.
    If the `ConnectionString` parameter is not present, the cmdlet attempts to 
    use the global connection string if set.

    .PARAMETER Factory
    (Optional) The `DbProviderFactory` used to create the connection object.

    .PARAMETER ProviderName
    (Optional) The name of the provider used to create or get the DbProviderFactory
    if specified and the `Factory` parameter is not.

    .PARAMETER Do
    (Optional) If specified, the connection is opened, and two variables are bound to the
    script block, `$_` and `$Connection` which can be used within the script block and the 
    connection is closed and disposed after the script block executes.

    .EXAMPLE
    $connection = New-SqlConnection -ConnectionString "Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True"

    .EXAMPLE
    New-SqlConnection $connectionString -Do { Write-Host ($_.State)  }

    .EXAMPLE
    $factory | New-SqlConnection -Do { Write-Host ($Connection.ConnectionString) }
#>
    
    [System.Diagnostics.CodeAnalysis.SuppressMessage("PsUseDeclaredVarsMoreThanAssignments", "")]
    Param(
        [Parameter(Position = 0)]
        [string] $ConnectionString,

        [Alias("cn")]
        [string] $ConnectionStringName,
        
        [Parameter(ValueFromPipeline = $True)]
        [System.Data.Common.DbProviderFactory] $Factory,
        
        [Alias("pn")]
        [string] $ProviderName = "Default",
        
        [ScriptBlock] $Do  
    )
   
    if(!$Factory) {
        
        $Factory = Get-GzDbProviderFactory -ProviderName $ProviderName
        
    }

    # name connection $c in order to create 
    # a new $Connection variable and bind it to the $Do script block
    $c = $Factory.CreateConnection();
    $hasConnectionString = ![string]::IsNullOrWhiteSpace($ConnectionString)
    if(!$hasConnectionString) {
        if(![string]::IsNullOrWhiteSpace($ConnectionStringName)) {
            $ConnectionString = Get-GzDbConnectionString -Name $Name 
            if([String]::IsNullOrWhiteSpace($ConnectionString)) {
                throw "Could not find connection string for $Name"
            }
        } else {
            $ConnectionString = Get-GzDbConnectionString 
        }

        
        $hasConnectionString = ![string]::IsNullOrWhiteSpace($ConnectionString)
    }
    if($hasConnectionString) {
        $c.ConnectionString = $ConnectionString;
    }

    if($Do) {
        if(!$hasConnectionString) {
            throw new [ArgumentNullException]("ConnectionString")
        }
        $c.Open();

        # Declared for Scope Below
        $Connection = $c;
        Set-Variable -Name "_" -Value $c 
        $vars = @(
            (Get-Variable -Name "Connection" -Scope 0)
            (Get-Variable -Name "_")
        )
        try {
            
            return $Do.InvokeWithContext(@{}, $vars)
        } finally {
            $c.Dispose()
        }
    }

    return $c;
}