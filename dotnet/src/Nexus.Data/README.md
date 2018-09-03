


```yaml

tables: 
  resources:
    columns:
      id: INT64 IDENTITY KEY
      type: STRING(256)
      key: INT NULL
      uri: STRING(2048) NULL 
    
  users:
    columns:
      id: INT IDENTITY KEY
      name: STRING(256) UNIQUE(ux_users_name)
      display_name: STRING(256) NULL
      is_banned: BOOLEAN DEFAULT(0)

  groups:
    columns:
      id: INT IDENTITY KEY
      name: STRING(256) UNIQUE(ux_groups_name)

```