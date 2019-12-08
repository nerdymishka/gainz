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

    public interface IRdbmsMigrationHistoryService : IMigrationHistoryService
    {
        ISqlExecutor SqlExecutor { get; set; }

        string SchemaName { get; }

        string TableName { get; }
    }

    public interface IMigrationHistoryService
    {

        long GetCurrentVersion(string category = null);

        bool HasStore();

        void CreateStore();

        void DropStore();

        void AddVersion(IMigrationVersion version);

        void RemoveVersion(IMigrationVersion version);

        void RemoveVersion(long version);
    }
}