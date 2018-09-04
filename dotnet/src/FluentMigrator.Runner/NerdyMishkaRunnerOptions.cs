namespace NerdyMishka.FluentMigrator.Runner
{
    public class NerdyMishkaRunnerOptions : INerdyMishkaRunnerOptions
    {
        public string DefaultSchema { get; set; }

        public bool OwnsSchema { get; set; }
    }
}