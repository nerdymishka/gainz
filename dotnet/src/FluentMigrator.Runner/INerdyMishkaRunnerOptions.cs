namespace NerdyMishka.FluentMigrator.Runner
{
    public interface INerdyMishkaRunnerOptions
    {
        string DefaultSchema { get; set; }

        bool OwnsSchema { get; set;}

        string Provider { get; set; }
    }
}