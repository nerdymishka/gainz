using NerdyMishka.Data;

namespace NerdyMishka.Data.Migrations
{

    public interface IMigrationVersion
    {
        long Version { get; }

        string Description { get; }

        string Category { get; }

        string[] Tags { get; }
    }

    public interface IMigrationHistoryService
    {
        string SchemaName { get; }

        string TableName { get; }

        ISqlExecutor SqlExecutor { get; set; }

        long GetCurrentVersion(string category = null);

        void CreateTable();

        void DropTable();

        void AddVersion(IMigrationVersion version);

        void RemoveVersion(IMigrationVersion version);

        void RemoveVersion(long version);
    }
}