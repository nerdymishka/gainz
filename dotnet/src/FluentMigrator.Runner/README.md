# NerdyMishka.FluentMigrator.Runner

FluentMigrator is awesome, but its missing the ablity migrate based on modules/plugins.

This code enables modules for FluentMigrator. The version table will use more of traditional 
db naming style with lowercase / underscore conventions.

## VersionTable

```sql
CREATE TABLE [migrations] (
    [version] BIGINTEGER NOT NULL
    [description] VARCHAR(1024) NOT NULL
    [applied_at] DATETIME NOT NULL
    [module] VARCHAR(500) NOT NULL
)
```