using System.IO;
using System.Linq;

namespace NerdyMishka.Data.Migrations
{
    public abstract class MigrationBase : IMigration
    {

        protected MigrationBase()
        {
            this.Initialize();
        }

        protected virtual void Initialize()
        {
            var versionInfo  = this.GetType().GetCustomAttributes(true)
                .Where(o => o is IMigrationVersion)
                .Select(o => (IMigrationVersion)o)
                .FirstOrDefault();

            if(versionInfo != null)
            {
                this.Version = versionInfo.Version;
                this.Category = versionInfo.Category;
                this.Description = versionInfo.Description;
                this.Tags = versionInfo.Tags;
            }
        }

        public long Version { get; protected set; }

        public string Description { get; protected set; }

        public string Category { get; protected set; }

        public string[] Tags { get; protected set; }

        public ISqlExecutor SqlExecutor { get; set; }	

        protected int ExecuteSql(SqlBuilder builder)
        {
            return this.ExecuteSql(builder.ToString());
        }

        protected int ExecuteSql(string sql)
        {
            int aggregate = 0;
            var splitters = new string[] { "\nGO\r\n", "\nGo\r\n", "\nGO\n", "\nGo\n" };
            var splitSqls = sql.Split(splitters, System.StringSplitOptions.RemoveEmptyEntries);
            foreach (var splitSql in splitSqls)
            {
                aggregate += this.SqlExecutor.Execute(splitSql);
            }

            return aggregate;
        }

        protected int ExecuteEmbededSql(string resourceName)
        {
            var assembly = this.GetType().Assembly;
    
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                if (stream == null)
                {
                    throw new System.Exception("Resource not found: " + resourceName);
                }

                using (StreamReader reader = new StreamReader(stream))
                {
                    string script = reader.ReadToEnd();

                    return this.ExecuteSql(script);
                }
            }
        }

        public abstract void Up();

        public abstract void Down();
    }
}