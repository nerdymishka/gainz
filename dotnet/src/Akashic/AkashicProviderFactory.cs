using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace NerdyMishka.Data
{
    public static class AkashicProviderFactory
    {
        private static readonly List<ProviderInfo> assemblyInfos;

        public const string OleDb = "OleDb";
        public const string Odbc = "Odbc";
        public const string SqlServer = "SqlServer";
        public const string SqlServerCore = "SqlServerCore";
        public const string SqliteCore = "SqliteCore";
        public const string MySql = "MySql";
        public const string Postgres = "Postgres";

       

        static AkashicProviderFactory()
        {
            assemblyInfos = new List<ProviderInfo>();

            assemblyInfos.Add(new ProviderInfo()
            {
                Name = OleDb,
                Pattern = "System.Data, Version={0}, Culture=neutral, PublicKeyToken=b77a5c561934e089",
                Version = "4.0.0.0",
                ClassName = "System.Data.OleDb.OleDbFactory",
                FieldName = "Instance",
                
            });

            assemblyInfos.Add(new ProviderInfo()
            {
                Name = Odbc,
                Pattern = "System.Data, Version={0}, Culture=neutral, PublicKeyToken=b77a5c561934e089",
                Version = "4.0.0.0",
                ClassName = "System.Data.Odbc.OdbcFactory",
                FieldName = "Instance"
            });

            assemblyInfos.Add(new ProviderInfo()
            {
                Name = SqlServerCore,
                Pattern = "System.Data.SqlClient, Version={0}, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
                Version = "4.0.0.0",
                ClassName = "System.Data.SqlClient.SqlClientFactory",
                FieldName = "Instance",
                Dialect = new NerdyMishka.Data.SqlClient.SqlServerDialect()
            });

            assemblyInfos.Add(new ProviderInfo()
            {
                Name = SqlServer,
                Pattern = "System.Data, Version={0}, Culture=neutral, PublicKeyToken=b77a5c561934e089",
                Version = "4.0.0.0",
                ClassName = "System.Data.SqlClient.SqlClientFactory",
                FieldName = "Instance",
                Dialect = new SqlServerDialect()
            });

            assemblyInfos.Add(new ProviderInfo()
            {
                Name = MySql,
                Pattern = "MySqlConnector, Version={0}, Culture=neutral, PublicKeyToken=d33d3e53aa5f8c92",
                Version = "0.43.0",
                ClassName = "MySql.Data.MySqlClient.MySqlClientFactory"
            });


            assemblyInfos.Add(new ProviderInfo()
            {
                Name = Postgres,
                Pattern = "Npgsql, Version={0}, Culture=neutral, PublicKeyToken=5d8b90d52f46fda7",
                Version = "3.1.0.0",
                ClassName = "Npgsql.NpgsqlFactory"
            });

            assemblyInfos.Add(new ProviderInfo()
            {
                Name = SqliteCore,
                Pattern = "Microsoft.Data.Sqlite, Version={0}, Culture=neutral, PublicKeyToken=adb9793829ddae60",
                Version = "1.0.1.0",
                ClassName = "Microsoft.Data.Sqlite.SqliteFactory",
                FieldName = "Instance",
                Dialect = new NerdyMishka.Data.Sqlite.SqliteDialect()
            });
        }


        public static ProviderInfo GetProviderInfo(string name)
        {
            var normalizedName = name.ToLower();
            return assemblyInfos.SingleOrDefault(o => o.Name.ToLower() == normalizedName);
        }

        public static void AddOrUpdate(ProviderInfo info)
        {
            var currentInfo = GetProviderInfo(info.Name.ToLower());
            if(currentInfo != null)
            {
                assemblyInfos.Remove(currentInfo);
            }

            assemblyInfos.Add(info);
        }

        internal static DbProviderFactory GetDbFactory(ProviderInfo providerInfo)
        {
            if (providerInfo.Instance != null)
                return providerInfo.Instance;

            var assemblyName = new AssemblyName(string.Format(providerInfo.Pattern, providerInfo.Version));
            var assembly = Assembly.Load(assemblyName);

            if (providerInfo.FieldName != null)
            {
                var field = assembly.LoadField(providerInfo.ClassName, providerInfo.FieldName);
                if (field == null)
                    throw new TypeLoadException($"{providerInfo.ClassName}.${providerInfo.FieldName} could not be loaded");

                providerInfo.Instance = (DbProviderFactory)field.GetValue(null);
            }
            else
            {
                providerInfo.Instance = (DbProviderFactory)Activator.CreateInstance(assembly.LoadClass(providerInfo.ClassName));
            }

            return providerInfo.Instance;
        }

        internal static DbProviderFactory GetDbFactory(string name)
        {
            var providerInfo = GetProviderInfo(name);
            if (providerInfo == null)
                throw new ArgumentException("A provider could not be found for " + name, name);

            return GetDbFactory(providerInfo);
        }

        public static AkashicFactory GetFactory(string name)
        {
            return new AkashicFactory(GetProviderInfo(name));
        }

    }
}
