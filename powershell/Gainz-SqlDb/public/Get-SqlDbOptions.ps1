$gainzSqlDbOptions = @{
    ConnectionStrings = @{
        Default = $null
    }
    DbProviderFactoryNames= @{
        SqlServer = "System.Data.SqlClient.SqlClientFactory"
        MsSql = "System.Data.SqlClient.SqlClientFactory"
    }
    DbProviderFactories = @{
        SqlServer = [System.Data.SqlClient.SqlClientFactory]::Instance
        MsSql = [System.Data.SqlClient.SqlClientFactory]::Instance
        Default = [System.Data.SqlClient.SqlClientFactory]::Instance
    }
}


function Get-SqlDbOption() {
    <#
    .SYNOPSIS
        Gets one or all options/configuraiton values for SqlDb Module.  
    
    .DESCRIPTION
        SqlDbOption holds connection strings and provider factory strings which are accessible
        via path strings. e.g.  "DbProviderFactories/SqlServer", "ConnectionStrings/Default"

    .PARAMETER Path
        Optional. If the path is specified, the value is returned. Otherwise all the values are returned
        as a hashable.  Aliases: Name
    
    .EXAMPLE
        PS C:\> Get-SqlDbOptions "ConnectionStrings/Default"
        REturns the default connection string value.

    .NOTES
        Additional values can be added using Set-SqDbOption -Path "MyValue" -Value "Value"
    #>
    [CmdletBinding()]
    Param(
        [Alias("Name")]
        [String] $Path
    )

    if(!$Path) {
        return $gainzSqlDbOptions;
    }
    $segments = @($Path);

    if($Path.Contains(".")) { $segments = $Path.Split("."); }
    if($Path.Contains("/")) { $segments = $Path.Split("/"); }

    $root = $gainzSqlDbOptions
    foreach($segment in $segments) {
        $root = $root[$segment];
    }

    return $root;
}


function Set-SqlDbOption() {
    <#
    .SYNOPSIS
        Sets a value for a sql db option/configuratio value.  

    .DESCRIPTION
        Sets a config value using a name/path value that is stored in memory 
        for this module. 

    .PARAMETER Path
        Reqiured. The path to the configuration option. Paths can use forward slashes or dots
        as a delimiter. Aliases: Name

    .PARAMETER Value
        Required. The value for the configuration option. 'ConfigurationStrings` and 'DbProviderFactories'
        must be a hashtable object.

    .EXAMPLE
        PS C:\> Set-SqlDbOption -Name "ConnectionStrings/Default" -Value "DataStore=:memory:"
        Sets a configuration value.
    #>
    [CmdletBinding()]
    Param(
        [Alias("Name")]
        [String] $Path,

        [Object] $Value
    )


    while([string]::IsNullOrWhiteSpace($Path)) {
        $Path = Read-Host "DbOption Path e.g. ConnectionStrings/MyApp ?"
    }

    if($Path -eq "ConnectionStrings") {
        if(! ($Value -is [Hashtable])) {
            Write-Warning "ConnectionStrings must be a hastable"
            return;
        }
    }

    if($Path -eq "DbProviderFactories") {
        if(! ($Value -is [Hashtable])) {
            Write-Warning "DbPRoviderFactories must be a hastable"
            return;
        }
    }

    if($Path -eq "DbProviderFactoryNames") {
        if(! ($Value -is [Hashtable])) {
            Write-Warning "DbPRoviderFactoryNames must be a hastable"
            return;
        }
    }

    $segments = @($Path);

    if($Path.Contains(".")) { $segments = $Path.Split("."); }
    if($Path.Contains("/")) { $segments = $Path.Split("/"); }

    $root = $gainzSqlDbOptions
    if(!$root) {
        Write-Warning "missing gainzSqlDbOptions"
    }
    for($i = 0; $i -lt $segments.Length; $i++) {
        $segment = $segments[$i]
        if($i -eq ($segments.Length - 1)) {
            if($root.ContainsKey($segment)) {
                $root[$segment] = $Value
            } else {
                $root.Add($segment, $Value);
            }
            break;
        }

        if($root.ContainsKey($segment)) {
            $root = $root[$segment];
            if(! ($root -is [Hashtable])) {
                Write-Warning "$segment is not a hashable"
                return;
            }
        } else {
            $root.Add($segment, @{})
            $root = $root[$segment];
        }
    }
}