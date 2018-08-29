Get-Item "$PsScriptRoot\..\public\*.ps1" | ForEach-Object {
    . "$($_.FullName)"
}



$masterConnectionString = "Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True";
$connectionString = "$masterConnectionString;Initial Catalog=FMG"
$diskDir = "$Env:TEMP\Gainz-SqlDb"
$createDb =  "
    CREATE DATABASE
            [FMG]
        ON PRIMARY (
        NAME=Fmg,
        FILENAME = '$diskDir\FMG.mdf'
        )
        LOG ON (
            NAME=Fmg_log,
            FILENAME = '$diskDir\FMG_log.ldf'
        )
"

    #SETUP


if(!(Test-Path $diskDir)) {
     mkdir $diskDir -Force
}

Describe "Gainz-SqlDb" {

    Context "Get-DbProviderFactory" {
        It "should return SqlClientFactory by default" {
            $factory = Get-DbProviderFactory 
            $factory | Should Not Be $Null
            $factory.ToString() | Should Be "System.Data.SqlClient.SqlClientFactory"
        }
    }

    Context "Get-DbConnectionString" {
        It "should be null by default" {
            $cs = Get-DbConnectionString 
            $cs | Should Be $Null 
        }

        It "should return the set value" {
            Set-DbConnectionString $connectionString 
            $cs = Get-DbConnectionString
            $cs | Should Be $connectionString 
            $factory = Get-DbProviderFactory
            $factory | Should Not Be $Null
            $factory.ToString() | Should Be "System.Data.SqlClient.SqlClientFactory"
        }
    }

    Context "Get-DbParameterPrefix" {
        It "should be the @ symbol by default" {
            $prefix = Get-DbParameterPrefix 
            $prefix | Should Not Be $Null 
            $prefix | Should Be "@"
        }
    }

    if($Env:OS -ne "Windows_NT") {

        return;
    }

    Context "New-DbConnection" {
        It "should return an object when no script block is present" {
            $connection = New-DbConnection $masterConnectionString
            $connection | Should Not Be $Null 
            $connection.State | Should Be "Closed"
        }

        It "Should open connection for script block" {
            $result = New-DbConnection $masterConnectionString -Do {
                $Connection | Should Not Be $Null
                $_ | Should Not Be $Null 
                $_.State | Should Be "Open"
            }

            $result | should not be $null
        }
    }

    
    Context "New-DbCommand" {
        It "Should create a command object" {
            New-DbConnection $masterConnectionString -Do {
                $cmd = $_ | New-DbCommand "Select 10"
                $cmd | Should Not Be $Null 
                $cmd.CommandText | Should Be "Select 10"
                $cmd.CommandType | Should Be "Text" 
                $cmd.Dispose();
            }
        }

        It "Should create a command object and bind it to a script block" {
            New-DbConnection $masterConnectionString -Do {
                # $Connection
                # $_ 
                $Connection | New-DbCommand "Select 10" -Do {
                    # $Command
                    # $_ 
                    $_ | Should Not Be $Null
                    $_.CommandText | Should Be "Select 10"
                }

                $_.ToString() | Should Be "System.Data.SqlClient.SqlConnection"
            }
        }

        It "Should add and bind parameters " {
            New-DbConnection $masterConnectionString -Do {
                # $Connection
                # $_ 
        
                $Connection | New-DbCommand "Select @Num" -Parameters @{"Num" = 10} -Do {
                    # $Command
                    # $_ 
                    $_ | Should Not Be $Null
                    $_.ToString() | Should Be "System.Data.SqlClient.SqlCommand"
                    $_.Parameters.Count | Should Be 1 
                    $_.Parameters["@Num"].Value | Should Be 10
                }

                $_.ToString() | Should Be "System.Data.SqlClient.SqlConnection"
            }
        }
    }

    Context "Read-DbData" {
        
        IT "Should select a value" {
        $data = Read-DbData "Select 10 As [TestColumn]" -ConnectionString $masterConnectionString
        $data.TestColumn | Should Be 10
        }

        IT "Should bind to connection from pipeline" {
            New-DbConnection $masterConnectionString -Do {
                $data = $_ | Read-DbData "Select @Num As [TestColumn1]" -Parameters @{"Num" = 11}
                $data | Should Not Be $Null 
                $data.TestColumn1 | Should Be 11
            }
        }
    }

    Context "Invoke-DbCmd" {
        It "Should invoke a nonquery" {
            
            $dbExists = Test-Path "$diskDir/fmg.mdf" 
            if($dbExists) {
                Invoke-DbCmd "ALTER DATABASE FMG SET SINGLE_USER WITH ROLLBACK IMMEDIATE;" -ConnectionString $masterConnectionString
                Invoke-DbCmd "DROP DATABASE FMG" -ConnectionString $masterConnectionString
            }
            $dbExists = Test-Path "$diskDir/fmg.mdf" 
            $dbExists | Should Be $False 

            Invoke-DbCmd $createDb -ConnectionString $masterConnectionString
            $dbExists = Test-Path "$diskDir/fmg.mdf" 
            $dbExists | Should Be $True

            $table = "CREATE TABLE test (
                id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
                FirstName nvarchar(255) NULL,
                LastName nvarchar(255) NULL
            );"

            $result = Invoke-DbCmd $table -ConnectionString $connectionString
        }
    }

    Context "Write-DbData" {
        It "Should write multiple records  to a table" {
            $set = @(
                @{"FirstName" = "Bob"; "LastName" = "Hernandez"},
                @{"FirstName" = "Princess"; "LastName" = "Zelda"}
            )
            $results = Write-DbData "Insert INTO test (FirstName,LastName) VALUES (@FirstName, @LastName)" -Parameters $set -ConnectionString $connectionString
            $data = Read-DbData "Select * FROM test" -ConnectionString $connectionString
            $data | Should NOT BE NULL 
            $data.Length |  Should Be 2
            $data[0].FirstName | Should Be "Bob";
        }
        
        It "Should write a single record to a table" {

            $results = Write-DbData "Insert INTO test (FirstName,LastName) VALUES (@FirstName, @LastName)" `
                    -Parameters  @{"FirstName" = "Link"; "LastName" = ""} `
                    -ConnectionString $connectionString

            $data = Read-DbData "Select * FROM test" -ConnectionString $connectionString
            $data | Should NOT BE NULL 
            $data.Length |  Should Be 3
            $data[2].FirstName | Should Be "Link";
        }

    }


# CLEAN UP
try {
    $dbExists = Test-Path "$diskDir/fmg.mdf" 
    if($dbExists) {
        Invoke-DbCmd "ALTER DATABASE FMG SET SINGLE_USER WITH ROLLBACK IMMEDIATE;" -ConnectionString $masterConnectionString
        Invoke-DbCmd "DROP DATABASE FMG" -ConnectionString $masterConnectionString
    }
    
    if(Test-Path $diskDir) {
        Remove-Item $diskDir -Force -Recurse
    }
} catch {
    
}

}
