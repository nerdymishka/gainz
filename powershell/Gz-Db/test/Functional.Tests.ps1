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

    Context "Get-GzDbProviderFactory" {
        It "should return SqlClientFactory by default" {
            $factory = Get-GzDbProviderFactory 
            $factory | Should Not Be $Null
            $factory.ToString() | Should Be "System.Data.SqlClient.SqlClientFactory"
        }

        It "should return SqliteFactory with named value" {
            $factory = Get-GzDbProviderFactory "Sqlite"
            $factory | Should Not Be $null
            $factory.ToString() | Should Be "System.Data.Sqlite.SqliteFactory"
        }
    }

    Context "Get-GzDbConnectionString" {
        It "should be null by default" {
            $cs = Get-GzDbConnectionString 
            $cs | Should Be $Null 
        }

        It "should return the set value" {
            Set-GzDbConnectionString $connectionString 
            $cs = Get-GzDbConnectionString
            $cs | Should Be $connectionString 
            $factory = Get-GzDbProviderFactory
            $factory | Should Not Be $Null
            $factory.ToString() | Should Be "System.Data.SqlClient.SqlClientFactory"
        }
    }

    Context "Get-DbParameterPrefix" {
        It "should be the @ symbol by default" {
            $prefix = Get-GzDbParameterPrefix 
            $prefix | Should Not Be $Null 
            $prefix | Should Be "@"
        }
    }

    


    Context "Sqlite:New-GzDbCommand" {
        It "Should create a command object" {
            New-GzDbConnection "Data Source=:memory:" -pn "Sqlite" -Do {
                $cmd = $_ | New-GzDbCommand "Select 10"
                $cmd | Should Not Be $Null 
                $cmd.CommandText | Should Be "Select 10"
                $cmd.CommandType | Should Be "Text" 
                $cmd.Dispose();
            }
        }

        It "Should create a command object and bind it to a script block" {
            New-GzDbConnection "Data Source=:memory:" -pn "Sqlite" -Do {
                # $Connection
                # $_ 
                $Connection | New-GzDbCommand "Select 10" -Do {
                    # $Command
                    # $_ 
                    $_ | Should Not Be $Null
                    $_.CommandText | Should Be "Select 10"
                }

                $Connection.ToString() | Should Be "System.Data.SQLite.SQLiteConnection"
            }
        }

        It "Should add and bind parameters " {
            New-GzDbConnection "Data Source=:memory:" -pn "Sqlite" -Do {
                # $Connection
                # $_ 
        
                $Connection | New-GzDbCommand "Select @Num" -Parameters @{"Num" = 10} -Do {
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

    Context "New-GzDbConnection" {
        It "should return an object when no script block is present" {
            $connection = New-GzDbConnection $masterConnectionString
            $connection | Should Not Be $Null 
            $connection.State | Should Be "Closed"
        }

        It "Should open connection for script block" {
            $result = New-GzDbConnection $masterConnectionString -Do {
                $Connection | Should Not Be $Null
                $_ | Should Not Be $Null 
                $_.State | Should Be "Open"
            }

            $result | should be $null

            $result = New-GzDbConnection $masterConnectionString -Do {
                $Connection | Should Not Be $Null
                $_ | Should Not Be $Null 
                $_.State | Should Be "Open"

                return "test"
            }

            $result | should be "test"
        }
    }

    
    Context "New-GzDbCommand" {
        It "Should create a command object" {
            New-GzDbConnection $masterConnectionString -Do {
                $cmd = $_ | New-GzDbCommand "Select 10"
                $cmd | Should Not Be $Null 
                $cmd.CommandText | Should Be "Select 10"
                $cmd.CommandType | Should Be "Text" 
                $cmd.Dispose();
            }
        }

        It "Should create a command object and bind it to a script block" {
            New-GzDbConnection $masterConnectionString -Do {
                # $Connection
                # $_ 
                $Connection | New-GzDbCommand "Select 10" -Do {
                    # $Command
                    # $_ 
                    $_ | Should Not Be $Null
                    $_.CommandText | Should Be "Select 10"
                }

                $_.ToString() | Should Be "System.Data.SqlClient.SqlConnection"
            }
        }

        It "Should add and bind parameters " {
            New-GzDbConnection $masterConnectionString -Do {
                # $Connection
                # $_ 
        
                $Connection | New-GzDbCommand "Select @Num" -Parameters @{"Num" = 10} -Do {
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

    Context "Read-GzDbData" {
        
        IT "Should select a value" {
        $data = Read-GzDbData "Select 10 As [TestColumn]" -ConnectionString $masterConnectionString
        $data.TestColumn | Should Be 10
        }

        IT "Should bind to connection from pipeline" {
            New-GzDbConnection $masterConnectionString -Do {
                $data = $_ | Read-DbData "Select @Num As [TestColumn1]" -Parameters @{"Num" = 11}
                $data | Should Not Be $Null 
                $data.TestColumn1 | Should Be 11
            }
        }
    }

    Context "Invoke-GzDbCommand" {
        It "Should invoke a nonquery" {
            
            $dbExists = Test-Path "$diskDir/Gz_Test.mdf" 
            if($dbExists) {
                Invoke-GzDbCommand "ALTER DATABASE FMG SET SINGLE_USER WITH ROLLBACK IMMEDIATE;" -ConnectionString $masterConnectionString
                Invoke-GzDbCommand "DROP DATABASE FMG" -ConnectionString $masterConnectionString
            }
            $dbExists = Test-Path "$diskDir/Gz_Test.mdf" 
            $dbExists | Should Be $False 

            Invoke-GzDbCommand $createDb -ConnectionString $masterConnectionString
            $dbExists = Test-Path "$diskDir/Gz_Test.mdf" 
            $dbExists | Should Be $True

            $table = "CREATE TABLE test (
                id INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
                FirstName nvarchar(255) NULL,
                LastName nvarchar(255) NULL
            );"

            $result = Invoke-GzDbCommand $table -ConnectionString $connectionString
            $result | Should Not Be $Null
        }
    }

    Context "Write-GzDbData" {
        It "Should write multiple records  to a table" {
            $set = @(
                @{"FirstName" = "Bob"; "LastName" = "Hernandez"},
                @{"FirstName" = "Princess"; "LastName" = "Zelda"}
            )
            Write-GzDbData "Insert INTO test (FirstName,LastName) VALUES (@FirstName, @LastName)" -Parameters $set -ConnectionString $connectionString
            $results | Should Be $Null 
            $data = Read-GzDbData "Select * FROM test" -ConnectionString $connectionString
            $data | Should NOT BE $NULL 
            $data.Length |  Should Be 2
            $data[0].FirstName | Should Be "Bob";
        }
        
        It "Should write a single record to a table" {
            Add-GzDbAlias 
            Write-DbData "Insert INTO test (FirstName,LastName) VALUES (@FirstName, @LastName)" `
                    -Parameters  @{"FirstName" = "Link"; "LastName" = ""} `
                    -ConnectionString $connectionString
          
            $data = Read-DbData "Select * FROM test" -ConnectionString $connectionString
            $data | Should NOT BE NULL 
            $data.Length |  Should Be 3
            $data[2].FirstName | Should Be "Link";
            Remove-GzDbAlias
        }

    }


# CLEAN UP
try {
    $dbExists = Test-Path "$diskDir/Gz_Test.mdf" 
    if($dbExists) {
        Invoke-GzDbCommand "ALTER DATABASE Gz_Test SET SINGLE_USER WITH ROLLBACK IMMEDIATE;" -ConnectionString $masterConnectionString
        Invoke-GzDbCommand "DROP DATABASE Gz_Test" -ConnectionString $masterConnectionString
    }
    
    if(Test-Path $diskDir) {
        Remove-Item $diskDir -Force -Recurse
    }
} catch {
    
}

}
