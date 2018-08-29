# Gainz SqlDb

Database agnostic Powershell functions for reading and writing data to SQL databases and invoking commands.

- Write-DbData - inserts or updates data in the database.
- Read-DbData - reads data from the database.
- Invoke-DbCmd - executes a statement such as create database or grants.

```powershell
Set-DbConnectionString "Server=localhost;Database=test;Integrate Security=true" -Name "Default"

# uses the default connection string set above 
$data = Read-DbData "SELECT name FROM [users]"
Write-Host $data  

# control the connection
$connection = New-DbConnection -ConnectionString $cs
$connection.Open()

$emails = $connection | Read-DbData "SELECT email FROM [users]"
$connection | Write-DbData 'INSERT [name] INTO [user_roles] ([name], [role]) VALUES (@name, 1)' -Parameters @{name = 'test'}

$onnection.Dispose()
```