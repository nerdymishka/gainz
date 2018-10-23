using System;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace NerdyMishka.Data
{
    public class SqlServerOperations 
    {
        private SqlBuilder builder = new SqlBuilder(new SqlServerDialect());

        private DataConnection connection;


        

        public void CreateDatabase(string name, AzureSqlDbCreateOptions options)
        {
            var builder = this.CreateDatabaseBuilder(name, options);

            this.connection.Execute(this.builder);
        }

        public async Task<int> CreateDatabaseAsync(string name, AzureSqlDbCreateOptions options)
        {
            var builder = this.CreateDatabaseBuilder(name, options);
            return await this.connection.ExecuteAsync(builder);
        }

        public void CreateUser(string name, SqlUserCreateOptions options)
        {
            DataConnection childConnection = null;

            var builder = this.CreateLoginBuilder(name, options);
            if(builder != null)
            {
                this.connection.Execute(builder);
            }

            builder = this.CreateUserBuilder(name, options);

            var sql = builder.ToString(true);
            var connectionString = new SqlConnectionStringBuilder();
            
            foreach(var database in options.Databases)
            {
                connectionString.InitialCatalog = database;
                using(childConnection = new DataConnection(KnownProviders.SqlServer, connectionString.ToString()))
                {
                    childConnection.Execute(sql);
                }
            }
        }

       

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

        protected SqlBuilder CreateRoleBuilder(string name)
        {
            var builder = new SqlBuilder();

            return builder.Append("CREATE SERVER ROLE ").Append(name);
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

        protected virtual SqlBuilder CreateDatabaseBuilder(string name, AzureSqlDbCreateOptions options)
        {
            var builder = new SqlBuilder();
            builder.Append("CREATE DATABASE ")
                .AppendIdentifier(name);

            if(!string.IsNullOrWhiteSpace(options.SourceDatabase))
            {
                builder.Append("AS COPY OF ")
                    .AppendIdentifier(options.SourceDatabase);
            }

            if(!string.IsNullOrWhiteSpace(options.ElasticPoolName) &&
                !string.IsNullOrWhiteSpace(options.ServiceTier)) {
                throw new ArgumentException("ServiceTier and ElasticPoolName are excluse. Choose one.");
            }

            var miniBuilder = new SqlBuilder();
            var hasOption = false;

            if(!string.IsNullOrWhiteSpace(options.MaxSize))
            {
                 miniBuilder.Append("MAXSIZE = ")
                    .Quote(options.MaxSize);

                hasOption = true;
            }


            if(!string.IsNullOrWhiteSpace(options.ElasticPoolName)) {
                if(hasOption)
                    miniBuilder.Append(", ");

                miniBuilder.Append("SERVICE_OBJECTIVE = ELASTIC_POOL( name = ")
                    .AppendIdentifier(options.ElasticPoolName)
                    .Append(")");

                 hasOption = true;
            }

            if(!string.IsNullOrWhiteSpace(options.Edition)) {
                if(hasOption)
                    miniBuilder.Append(", ");

                miniBuilder.Append("EDITION = ")
                    .Quote(options.Edition);

                hasOption = true;
            }

            if(!string.IsNullOrWhiteSpace(options.ServiceTier)) {
                if(hasOption)
                    miniBuilder.Append(", ");

                miniBuilder.Append("SERVICE_OBJECTIVE = ")
                    .Quote(options.ServiceTier);

                hasOption = true;
            }

            if(hasOption) {
                builder.Append("( ")
                    .Append(miniBuilder)
                    .Append(" )");
            }

            return builder;
        }

        public int CreateDatabase(string name, SqlDbCreateOptions options)
        {
            var builder = this.CreateDatabaseBuilder(name, options);

            return this.connection.Execute(builder);
        }

        public async Task<int> CreateDatabaseAsync(string name, SqlDbCreateOptions options)
        {
            var builder = this.CreateDatabaseBuilder(name, options);

            return await this.connection.ExecuteAsync(name)
                .ConfigureAwait(false);
        }

        
        protected virtual SqlBuilder CreateDatabaseBuilder(string name, SqlDbCreateOptions options)
        {
            var builder = new SqlBuilder();

            builder.Append("CREATE DATABASE ")
                .Append(name);

            if(options == null)
                return builder;

            if(! string.IsNullOrWhiteSpace(options.Directory)) {
                builder.AppendLine("ON PRIMARY (")
                       .AppendLine($"    NAME={name}")
                       .AppendLine($"    FILENAME = '{options.Directory}\\{name}.mdf'")
                       .AppendLine(")")
                       .AppendLine("LOG ON (")
                       .AppendLine($"   NAME={name}_log")
                       .AppendLine($"   FILENAME = '{options.Directory}\\{name}.ldf'")
                       .AppendLine(")");

                if(options.Attach) {
                    builder.AppendLine("FOR ATTACH");
                }
            }

            return builder;
        }
    }
}
