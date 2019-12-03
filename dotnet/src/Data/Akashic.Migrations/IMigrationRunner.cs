
namespace NerdyMishka.Data.Migrations
{
    public interface IMigrationRunner
    {
        void StepToVersion(int steps, string category = null);

        void RunToVersion(long version, string category = null);

        void Run(string category = null);
    }
}