using System;
using FluentMigrator;
using FluentMigrator.Builders.Create;
using FluentMigrator.Builders.Create.Table;

namespace NerdyMishka.FluentMigrator
{
    public static class NerdyMishkaMigrationExtensions
    {
        public static string DefaultSchema { get; set;} = "nexus";
        public static bool UseDefaultSchemaForVersionTable { get; set;} = true;

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
                                    .AsString();
                    } else {
                        next = syntax.WithColumn(name)
                                    .AsString(limit);
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

        public static ICreateTableWithColumnSyntax CreateTable(this Migration migration, string table, string schema = null) 
        {
            schema = DefaultSchema;

            if(string.IsNullOrWhiteSpace(schema))
                return migration.Create.Table(table);

            return migration.Create.Table(table).InSchema(schema);
        }

        public static Migration DropTables(this Migration migration, string[] tables)
        {
            return DropTables(migration, DefaultSchema, tables);
        }

        public static Migration DropTables(this Migration migration, string schema, string[] tables)
        {
            if(string.IsNullOrWhiteSpace(schema))
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
            return GrantRolePermissionsToTable(migration, role, permissions, DefaultSchema, tables);
        }

        public static Migration GrantRolePermissionsToTable(
            this Migration migration, 
            string role,
            string[] tables)
        {
            return GrantRolePermissionsToTable(migration, role, null, DefaultSchema, tables);
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