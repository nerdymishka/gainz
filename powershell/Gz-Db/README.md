# Gainz SqlDb

Database agnostic Powershell functions for reading and writing data to SQL databases and invoking commands.

- Set-GzDbConnectionString - sets the default or a named connection string.
- New-GzDbConnection - creates a new connection
- Write-GzDbData - inserts or updates data in the database.
- Read-GzDbData - reads data from the database.
- Invoke-GzDbCommand - executes a statement such as create database or grants.

```powershell
Set-GzDbConnectionString "Server=localhost;Database=test;Integrate Security=true" -Name "Default"

# uses the default connection string set above 
$data = Read-GzDbData "SELECT name FROM [users]"
Write-Host $data  

# control the connection
$connection = New-GzDbConnection -ConnectionString $cs
$connection.Open()

$emails = $connection | Read-GzDbData "SELECT email FROM [users]"
$connection | Write-GzDbData 'INSERT [name] INTO [user_roles] ([name], [role]) VALUES (@name, 1)' -Parameters @{name = 'test'}

$onnection.Dispose()

# opens and closes the connection, returns any output.
$data = New-GzDbConnection -Do {
   return  $_ | Read-GzDbData "SELECT email FROM [users]"
}

```