using System;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace NerdyMishka.Data
{
    public class SqlDbOperations 
    {
        private SqlBuilder builder = new SqlBuilder(new SqlServerDialect());

        private DataConnection connection;

        public void CreateRole(string name)
        {
            this.connection.Execute(
                this.CreateRoleBuilder(name));
        }

        public async Task CreateRoleAsync(string name)
        {
            await this.connection.ExecuteAsync(
                this.CreateRoleBuilder(name))
                .ConfigureAwait(false);
        }

        protected SqlBuilder CreateTableBuilder(Action<DbTable> create)
        {
            return null;
        }

        protected SqlBuilder DropTableBuilder(string name, string schema = null)
        {
             var builder = new SqlBuilder();
             builder.Append("DROP TABLE IF EXISTS ");
             if(schema != null)
                builder.AppendIdentifiers(schema, name);
            else 
                builder.AppendIdentifier(name);

            return builder;
        }

        protected SqlBuilder DropFunctionBuilder(string name, string schema = null)
        {
             var builder = new SqlBuilder();
             builder.Append("DROP FUNCTION IF EXISTS ");
             if(schema != null)
                builder.AppendIdentifiers(schema, name);
            else 
                builder.AppendIdentifier(name);

            return builder;
        }

        protected SqlBuilder DropViewBuilder(string name, string schema = null)
        {
             var builder = new SqlBuilder();
             builder.Append("DROP VIEW IF EXISTS ");
             if(schema != null)
                builder.AppendIdentifiers(schema, name);
            else 
                builder.AppendIdentifier(name);

            return builder;
        }

        protected SqlBuilder DropStoredProcedureBuilder(string name, string schema = null)
        {
             var builder = new SqlBuilder();
             builder.Append("DROP PROCEDURE IF EXISTS ");
             if(schema != null)
                builder.AppendIdentifiers(schema, name);
            else 
                builder.AppendIdentifier(name);

            return builder;
        }

        protected SqlBuilder DropRoleBuilder(string name)
        {
            var builder = new SqlBuilder();

            return builder.Append("DROP ROLE IF EXISTS ").Append(name);
        }

        protected SqlBuilder CreateRoleBuilder(string name)
        {
            var builder = new SqlBuilder();

            return builder.Append("CREATE ROLE ").Append(name);
        }

        protected SqlBuilder CreateSchemaBuilder(string name)
        {
            var builder = new SqlBuilder();

            return builder.Append("CREATE SCHEMA ").Append(name);
        }
        

        protected SqlBuilder DropSchemaBuilder(string name)
        {
              var builder = new SqlBuilder();

            return builder.Append("DROP SCHEMA IF EXISTS ").Append(name);
        }

        protected SqlBuilder AddRoleMemberBuilder(string roleName, string userName)
        {
            var builder = new SqlBuilder();

            return builder.Append("EXEC sp_addrolemember N'")
                .Append(roleName)
                .Append("', N'")
                .Append(userName)
                .Append("'");
        }

        protected SqlBuilder GrantBuilder(string[] permissions, string on, string to)
        {
            var builder = new SqlBuilder();

            return builder.Append("GRANT ")
                .Append(string.Join(",", permissions))
                .Append(" ON ")
                .Append(on)
                .Append(" TO ")
                .Append(to);
        }


        public async Task CreateUserAsync(string name, SqlUserCreateOptions options)
        {
            DataConnection childConnection = null;

            if(options.Databases == null || options.Databases.Count == 0)
                throw new ArgumentException("options.Databases must have at least one database.");

            var builder = this.CreateLoginBuilder(name, options);
            if(builder != null)
            {
                await this.connection.ExecuteAsync(builder);
            }

            builder = this.CreateUserBuilder(name, options);

            var sql = builder.ToString(true);
            var connectionString = new SqlConnectionStringBuilder(this.connection.ConnectionString);
            
            foreach(var database in options.Databases)
            {
                connectionString.InitialCatalog = database;
                using(childConnection = new DataConnection(KnownProviders.SqlServer, connectionString.ToString()))
                {
                    await childConnection.ExecuteAsync(sql);
                }
            }
        }

        protected virtual SqlBuilder CreateUserBuilder(string name, SqlUserCreateOptions options)
        {
            var builder = new SqlBuilder();
            var user = name;
            var login = options.Login;
            if(options.EnsureLogin)
                login = login ?? name;

            if(options.EnsureSuffix)
            {
                if(!user.EndsWith("_user") && !user.Contains("\\"))
                    user += "_user";

                 if(!string.IsNullOrWhiteSpace(login) && !login.EndsWith("_login") && !login.Contains("\\"))
                        login += "_login";
            }

            if(options.DropUserIfExists)
            {
                builder.Append("DROP USER IF EXISTS ")
                        .Append(user)
                        .Append(";");
            }

            builder.Append("CREATE USER ")
                .Append(user);

            if(!string.IsNullOrWhiteSpace(login))
            {
                builder.Append("FOR LOGIN ").Append(login);
            }

            if(!string.IsNullOrWhiteSpace(options.UserPassword))
            {
                builder.Append("WITH PASSWORD = ")
                    .Quote(options.UserPassword);
            }

            return builder;
        }

        protected virtual SqlBuilder CreateLoginBuilder(string name, SqlUserCreateOptions options)
        {
            

            if(!options.EnsureLogin && string.IsNullOrWhiteSpace(options.Login))
                return null;

            var builder = new SqlBuilder();
            var login = options.Login;

            if(options.EnsureLogin)
            {
                login = login ?? name;
            }
            if(options.EnsureSuffix)
            {
                if(!login.EndsWith("_login") && !login.Contains("\\"))
                    login += "_login";

                options.Login = login;
            }
            
            builder.Append("IF NOT EXISTS (SELECT * FROM sys.sql_logins WHERE name = ")
                .Quote(login)
                .AppendLine(")")
                .AppendLine("   BEGIN")
                .Append("        CREATE LOGIN ")
                .Append(login);

            if(!string.IsNullOrWhiteSpace(options.LoginPassword)) {
                builder.Append("WITH PASSWORD = ")
                    .Quote(options.LoginPassword);
            }  

            builder.AppendLine("")
                .AppendLine("END");

            return builder;
            

            return null;
        }

    }
}
