using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.Data
{
    public class AkashicFactory 
    {
        private ProviderInfo info;
        private DbProviderFactory factory;

        public AkashicFactory(ProviderInfo info)
        {
            this.info = info;
            this.factory = AkashicProviderFactory.GetDbFactory(info);
        }

        public DbProviderFactory DbFactory => this.factory;

        public SqlDialect SqlDialect => this.info.Dialect;

        public string Name => this.info.Name;

        public virtual SqlBuilder CreateSqlBuilder()
        {
            return new SqlBuilder(info.Dialect);
        }

        public virtual IDataConnection CreateConnection(string connectionString = null)
        {
            var connection = this.factory.CreateConnection();
            if (connectionString != null)
                connection.ConnectionString = connectionString;

            return new DataConnection(connection, this.SqlDialect);
        }

        public virtual IDataCommand CreateCommand()
        {
            return new DataCommand(this.factory.CreateCommand());
        }

        public DbParameter CreateParameter()
        {
            return this.factory.CreateParameter();
        }

        public DbConnectionStringBuilder CreateConnectionStringBuilder()
        {
            return this.factory.CreateConnectionStringBuilder();
        }
    }













}
