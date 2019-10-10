Test-ModuleManifest "$PsScriptRoot\..\Gz-Db.psd1"
Import-Module "$PsScriptRoot\..\Gz-Db.psd1" -Force



$masterConnectionString = "Data Source=(LocalDB)\MSSQLLocalDB;Integrated Security=True";
$connectionString = "$masterConnectionString;Initial Catalog=Gz_Test"
$diskDir = "$Env:TEMP\Gz-Db"
$createDb =  "
    CREATE DATABASE
            [Gz_Test]
        ON PRIMARY (
        NAME=Fmg,
        FILENAME = '$diskDir\Gz_Test.mdf'
        )
        LOG ON (
            NAME=Fmg_log,
            FILENAME = '$diskDir\Gz_Test.ldf'
        )
"

    #SETUP


if(!(Test-Path $diskDir)) {
     mkdir $diskDir -Force
}

Describe "Gz-Db" {

    Context "Get-DbProviderFactory" {
        It "should return SqlClientFactory by default" {
            $factory = Get-DbProviderFactory 
            $factory | Should Not Be $Null
            $factory.ToString() | Should Be "System.Data.SqlClient.SqlClientFactory"
        }

        It "should return SqliteFactory with named value" {
            $factory = Get-DbProviderFactory "Sqlite"
            $factory | Should Not Be $null
            $factory.ToString() | Should Be "System.Data.Sqlite.SqliteFactory"
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

    


    Context "Sqlite:New-DbCommand" {
        It "Should create a command object" {
            New-DbConnection "Data Source=:memory:" -pn "Sqlite" -Do {
                $cmd = $_ | New-DbCommand "Select 10"
                $cmd | Should Not Be $Null 
                $cmd.CommandText | Should Be "Select 10"
                $cmd.CommandType | Should Be "Text" 
                $cmd.Dispose();
            }
        }

        It "Should create a command object and bind it to a script block" {
            New-DbConnection "Data Source=:memory:" -pn "Sqlite" -Do {
                # $Connection
                # $_ 
                $Connection | New-DbCommand "Select 10" -Do {
                    # $Command
                    # $_ 
                    $_ | Should Not Be $Null
                    $_.CommandText | Should Be "Select 10"
                }

                $Connection.ToString() | Should Be "System.Data.SQLite.SQLiteConnection"
            }
        }

        It "Should add and bind parameters " {
            New-DbConnection "Data Source=:memory:" -pn "Sqlite" -Do {
                # $Connection
                # $_ 
        
                $Connection | New-DbCommand "Select @Num" -Parameters @{"Num" = 10} -Do {
                    # $Command
                    # $_ 
                    $_ | Should Not Be $Null
                    $_.ToString() | Should Be "System.Data.SQLite.SQLiteCommand"
                    $_.Parameters.Count | Should Be 1 
                    $_.Parameters["@Num"].Value | Should Be 10
                }

                $Connection.ToString() | Should Be "System.Data.SQLite.SQLiteConnection"
            }
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

            $result | should be $null

            $result = New-DbConnection $masterConnectionString -Do {
                $Connection | Should Not Be $Null
                $_ | Should Not Be $Null 
                $_.State | Should Be "Open"

                return "test"
            }

            $result | should be "test"
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

    Context "Invoke-DbCommand" {
        It "Should invoke a nonquery" {
            
            $dbExists = Test-Path "$diskDir/Gz_Test.mdf" 
            if($dbExists) {
                Invoke-DbCommand "ALTER DATABASE FMG SET SINGLE_USER WITH ROLLBACK IMMEDIATE;" -ConnectionString $masterConnectionString
                Invoke-DbCommand "DROP DATABASE FMG" -ConnectionString $masterConnectionString
            }
            $dbExists = Test-Path "$diskDir/Gz_Test.mdf" 
            $dbExists | Should Be $False 

            Invoke-DbCommand $createDb -ConnectionString $masterConnectionString
            $dbExists = Test-Path "$diskDir/Gz_Test.mdf" 
            $dbExists | Should Be $True

            $table = "CREATE TABLE test (
                id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
                FirstName nvarchar(255) NULL,
                LastName nvarchar(255) NULL
            );"

            $result = Invoke-DbCommand $table -ConnectionString $connectionString
            $result | Should Not Be $Null
        }
    }

    Context "Write-DbData" {
        It "Should write multiple records  to a table" {
            $set = @(
                @{"FirstName" = "Bob"; "LastName" = "Hernandez"},
                @{"FirstName" = "Princess"; "LastName" = "Zelda"}
            )
            Write-DbData "Insert INTO test (FirstName,LastName) VALUES (@FirstName, @LastName)" -Parameters $set -ConnectionString $connectionString
            $results | Should Be $Null 
            $data = Read-DbData "Select * FROM test" -ConnectionString $connectionString
            $data | Should NOT BE $NULL 
            $data.Length |  Should Be 2
            $data[0].FirstName | Should Be "Bob";
        }
        
        It "Should write a single record to a table" {
          
            Write-DbData "Insert INTO test (FirstName,LastName) VALUES (@FirstName, @LastName)" `
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
    $dbExists = Test-Path "$diskDir/Gz_Test.mdf" 
    if($dbExists) {
        Invoke-DbCommand "ALTER DATABASE Gz_Test SET SINGLE_USER WITH ROLLBACK IMMEDIATE;" -ConnectionString $masterConnectionString
        Invoke-DbCommand "DROP DATABASE Gz_Test" -ConnectionString $masterConnectionString
    }
    
    if(Test-Path $diskDir) {
        Remove-Item $diskDir -Force -Recurse
    }
} catch {
    
}

}
