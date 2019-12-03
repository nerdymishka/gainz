using System.Collections.Generic;

namespace NerdyMishka.Data.Migrations
{
    public abstract class DatabaseInitializer : IDatabaseInitializer
    {
        public IDataConnection Connection { get; set; }

        public AkashicFactory Factory { get; set; }

        // In most cases, you're better off overriding the
        // create method. 
        public virtual void Create(bool drop = false)
        {
            var cs = this.Connection.ConnectionString;
            var builder = this.Factory.CreateConnectionStringBuilder();
            string database = null;
            var matches = new List<string>() {
                "database",
                "initial catalog",
            };
            foreach(string key in builder.Keys)
            {
                if(matches.Contains(key.ToLower())) {
                    database = builder[key].ToString();
                    builder[key] = null;
                    break;
                }
            }
        

            if(Factory.Name.StartsWith("Sqlite"))
            {
                if(drop) 
                {
                    var file = builder["DataSource"] as string;
                    if(!string.IsNullOrWhiteSpace(file))
                    {
                        var fi = new System.IO.FileInfo(file);
                        if(fi.Exists)
                            fi.Delete();
                    }
                }

                // this will created when the connection opens
                return;
            }

            if(database == null)
                return;
            

            // connect to the default database
            using(var connection = Factory.CreateConnection(builder.ToString()))
            {
                connection.Open();
                var sb = Connection.CreateSqlBuilder();
                if(drop)
                {
                    if(Factory.Name.StartsWith("SqlClient") || Factory.Name.StartsWith("SqlServer"))
                    {
                        sb.Append("IF EXISTS(SELECT Count(name) FROM sys.databases where name = ").Quote(database).Append(")")
                            .AppendLine("BEGIN")
                            .Append("    ALTER DATABASE ")
                            .AppendIdentifier(database)
                            .Append(" SET SINGLE_USER WITH ROLLBACK IMMEDIATE")
                            .AppendLine("END");
                            
                        connection.Execute(sb.ToString(true));
                    }


                    sb.Append("DROP DATABASE IF EXISTS ");
                    sb.AppendIdentifier(database);
                    connection.Execute(sb.ToString(true));
                }

                sb.Append("CREATE DATABASE ");
                sb.AppendIdentifier(database);
                connection.Execute(sb.ToString(true));
            }   
        }
    }
}