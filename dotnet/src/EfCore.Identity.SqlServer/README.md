# README

```powershell
dotnet-ef migrations add Initial  --project .\EfCore.Identity.SqlServer\ `
    --startup-project .\Migrator\

dotnet-ef migrations remove  --project .\EfCore.Identity.SqlServer\ `
    --startup-project .\Migrator\
```
