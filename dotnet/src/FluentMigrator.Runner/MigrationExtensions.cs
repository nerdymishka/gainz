using System;
using FluentMigrator;
using FluentMigrator.Builders.Create;
using FluentMigrator.Builders.Create.Table;
using FluentMigrator.Builders.Insert;

namespace NerdyMishka.FluentMigrator
{
    public static class NerdyMishkaMigrationExtensions
    {
        public static string DefaultSchema { get; set;} = "nexus";
        public static bool UseDefaultSchemaForVersionTable { get; set;} = true;

        public static ICreateTableColumnOptionOrWithColumnSyntax GuidPk(
            this ICreateTableWithColumnSyntax syntax, string name = "id") {

            return syntax
                    .WithColumn(name)
                    .AsGuid()
                    .PrimaryKey();
        }

        public static ICreateTableColumnOptionOrWithColumnSyntax LongPk(
            this ICreateTableWithColumnSyntax syntax, string name = "id") {

            return syntax
                    .WithColumn(name)
                    .AsInt64()
                    .Identity()
                    .PrimaryKey();
        }
         

        public static ICreateTableColumnOptionOrWithColumnSyntax Pk(
            this ICreateTableWithColumnSyntax syntax, string name = "id") {

            return syntax
                    .WithColumn(name)
                    .AsInt32()
                    .Identity()
                    .PrimaryKey();
        }


        public static ICreateTableColumnOptionOrWithColumnSyntax Column<T>( this ICreateTableWithColumnSyntax syntax,
            string name, 
            bool isNullable = false, 
            string indexName = null,
            int limit = int.MaxValue,
            string uniqueIndexName = null,
            bool isPrimaryKey = false,
            bool isIdentity = false,
            int? scale = null         
            )
        {
            ICreateTableColumnOptionOrWithColumnSyntax next = null;

            var type = typeof(T);

            if(type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>)) {
                type = type.GetGenericArguments()[0];
                isNullable = true;
            }

            switch(type.FullName)
            {
                case "System.String":
                    if(limit < int.MaxValue)
                    {
                        next = syntax.WithColumn(name)
                                    .AsString(limit);
                    } else {
                        next = syntax.WithColumn(name)
                                    .AsString();
                    }
                    break;
                case "System.Int32":
                    next = syntax.WithColumn(name).AsInt32();
                    break;
                case "System.Int16":
                    next = syntax.WithColumn(name).AsInt16();
                    break;
                case "System.Int64":
                    next = syntax.WithColumn(name).AsInt64();
                    break;

                case "System.Decimal":
                    if(!scale.HasValue)
                        scale = 2;
                    if(limit > 20)
                        limit = 20;
                    next = syntax.WithColumn(name).AsDecimal(limit, scale.Value);
                    break;
                case "System.DateTime":
                    next = syntax.WithColumn(name).AsDateTime();
                    break;
                case "System.TimeSpan":
                    next = syntax.WithColumn(name).AsDateTimeOffset();
                    break;
                case "System.Guid":
                    next = syntax.WithColumn(name).AsGuid();
                    break;
                case "System.Boolean":
                    next = syntax.WithColumn(name).AsBoolean();
                    break;
                case "System.Byte[]":
                    if(limit < int.MaxValue)
                        next = syntax.WithColumn(name).AsBinary(limit);
                    else 
                        next = syntax.WithColumn(name).AsBinary();
                    break;
                case "System.Byte":
                    next = syntax.WithColumn(name).AsByte();
                    break;
                default:
                    throw new NotSupportedException($"{type.FullName} is not supported");

            }

            if(isNullable)
                next = next.Nullable();
            else 
                next = next.NotNullable();

            if(!string.IsNullOrWhiteSpace(indexName))
                next = next.Indexed(indexName);

            if(!string.IsNullOrWhiteSpace(uniqueIndexName))
                next = next.Unique(uniqueIndexName);

            if(isIdentity)
                next = next.Identity();

            if(isPrimaryKey)
                next = next.PrimaryKey();

            return next;
        }

        public static Migration DisableAutoIncement(this Migration migration, string schema = null,  string table = null)
        {
            if(table != null && table.Length > 0)
            {
                if(!string.IsNullOrWhiteSpace(schema))
                {

                        migration.IfDatabase("SqlServer").Execute.Sql($"SET Identity_Insert [{schema}].[{table}] ON");
                    
                    return migration;
                }


                    migration.IfDatabase("SqlServer").Execute.Sql($"SET Identity_Insert [{table}] ON");
                
                return migration;
                
            }

            return migration;
        }

        
        public static Migration EnableAutoIncement(this Migration migration, string schema = null, string table = null)
        {
            if(table != null && table.Length > 0)
            {
                if(!string.IsNullOrWhiteSpace(schema))
                {
      
                    migration.IfDatabase("SqlServer").Execute.Sql($"SET Identity_Insert [{schema}].[{table}] OFF");
                    
                    return migration;
                }


                    migration.IfDatabase("SqlServer").Execute.Sql($"SET Identity_Insert [{table}] OFF");
                
                return migration;
                
            }

            return migration;
        }

        public static ICreateTableWithColumnSyntax CreateTable(this Migration migration, string table, string schema = null) 
        {
            
            if(schema == null && migration is IMigrationWithServiceProvider) {
                schema = ((IMigrationWithServiceProvider)migration).DefaultSchemaName;
            }

            if(string.IsNullOrWhiteSpace(schema))
                return migration.Create.Table(table);

            return migration.Create.Table(table).InSchema(schema);
        }

        public static Migration DropTables(this Migration migration, string[] tables)
        {
            string schema = null;
            if(migration is IMigrationWithServiceProvider) {
                schema = ((IMigrationWithServiceProvider)migration).DefaultSchemaName;
            }

            return DropTables(migration, schema, tables);
        }

        public static IInsertDataSyntax Rows(this IInsertDataSyntax insert, params object[] rows)
        {
            if(rows != null && rows.Length > 0)
            {
                foreach(var row in rows)
                {
                    insert.Row(row);
                }
            }

            return insert;
        }

        public static IInsertDataSyntax InsertInto(this Migration migration, string table, string schema)
        {
            if(string.IsNullOrEmpty(schema))
            {
                return migration.Insert.IntoTable(table);
            } else {
                var cs = migration.ConnectionString.ToLowerInvariant();
                if(cs.Contains("data source") && (cs.Contains(":memory:") || cs.Contains(".db"))) {
                    return migration.Insert.IntoTable(table);
                }

                return migration.Insert.IntoTable(table).InSchema(schema);
            }
        }

        private static bool IsSqlite(Migration migration)
        {
            var cs = migration.ConnectionString.ToLowerInvariant();
            return cs.Contains("data source") && (cs.Contains(":memory:") || cs.Contains(".db"));
        }

        public static Migration DropTables(this Migration migration, string schema, string[] tables)
        {


            if(string.IsNullOrWhiteSpace(schema) || IsSqlite(migration))
            {
                foreach(var table in tables)
                    migration.Delete.Table(table);

                return migration;
            }

            foreach(var table in tables)
                migration.Delete.Table(table).InSchema(schema);

            return migration;
        }

        public static Migration DropRole(this Migration migration, string role)
        {
            migration.IfDatabase("SqlServer").Execute.Sql($"DROP ROLE {role}");
            migration.IfDatabase("Postgres").Execute.Sql($"DROP ROLE {role}");
            migration.IfDatabase("MySql").Execute.Sql($"DROP ROLE '{role}'@'%'");

            return migration;
        }


        public static Migration CreateRole(this Migration migration, string role)
        {
            migration.IfDatabase("SqlServer").Execute.Sql($"CREATE ROLE {role}");
            migration.IfDatabase("Postgres").Execute.Sql($"CREATE ROLE {role}");
            migration.IfDatabase("MySql").Execute.Sql($"CREATE ROLE '{role}'@'%'");

            return migration;
        }

        public static Migration GrantRolePermissionsToTable(
            this Migration migration, 
            string role,
            string schema, 
            string[] tables)
        {
            return GrantRolePermissionsToTable(migration, role, null, schema, tables);
        }

        public static Migration GrantRolePermissionsToTable(
            this Migration migration, 
            string role,
            string[] permissions, 
            string[] tables)
        {
            string schema = null;
            if(migration is IMigrationWithServiceProvider) {
                schema = ((IMigrationWithServiceProvider)migration).DefaultSchemaName;
            }
            

            return GrantRolePermissionsToTable(migration, role, permissions, schema, tables);
        }

        public static Migration GrantRolePermissionsToTable(
            this Migration migration, 
            string role,
            string[] tables)
        {
            string schema = null;
            if(migration is IMigrationWithServiceProvider) {
                schema = ((IMigrationWithServiceProvider)migration).DefaultSchemaName;
            }
            return GrantRolePermissionsToTable(migration, role, null, schema, tables);
        }

        public static Migration GrantRolePermissionsToTable(
            this Migration migration, 
            string role,
            string [] permissions,
            string schema,
            string[] tables)
        {
            if(permissions == null || permissions.Length == 0)
                permissions = new string[] {"crud"};

            string permissionSet = "";
            if(permissions == null || permissions.Length == 1) {
                var cmd = permissions[0].ToLowerInvariant();
                switch(cmd) {
                    case "crud":
                        permissionSet = "SELECT, INSERT, DELETE, UPDATE";
                        break;
                    case "all":
                        permissionSet = "SELECT, INSERT, DELETE, UPDATE, ALTER";
                        break;
                    default:
                        permissionSet = cmd.ToUpperInvariant();
                        break;
                }
            } else {
                permissionSet = string.Join(", ", permissions);
            }

            if(!string.IsNullOrWhiteSpace(schema))
            {
                foreach(var table in tables)
                {
                    migration.IfDatabase("SqlServer").Execute.Sql(
                        $"GRANT {permissionSet} ON [{schema}].[{table}] TO {role}");

                    migration.IfDatabase("MySql").Execute.Sql(
                        $"GRANT {permissionSet} ON {schema}.{table} TO '{role}'@'%'"
                    );

                    migration.IfDatabase("Postgres").Execute.Sql(
                        $"GRANT {permissionSet} ON {schema}.{table} TO {role}"
                    );
                }

                return migration;
            }
            
            foreach(var table in tables)
            {            
                migration.IfDatabase("SqlServer").Execute.Sql(
                        $"GRANT {permissionSet} ON [{table}] TO {role}");

                migration.IfDatabase("MySql").Execute.Sql(
                    $"GRANT {permissionSet} ON {table} TO '{role}'@'*'"
                );

                migration.IfDatabase("Postgres").Execute.Sql(
                    $"GRANT {permissionSet} ON {table} TO {role}"
                );
            }

            return migration;
        }
    }
}