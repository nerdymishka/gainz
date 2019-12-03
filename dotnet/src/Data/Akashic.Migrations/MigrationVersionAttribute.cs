namespace NerdyMishka.Data.Migrations
{
    [System.AttributeUsage(System.AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    sealed class MigrationVersionAttribute : System.Attribute, IMigrationVersion
    {
        public long Version { get; private set; }

        public string Category { get; private set; }

        public string Description {get; private set; }

        public string[] Tags { get; private set; }

        public MigrationVersionAttribute(long version, string category = null, string description = null, string tags = null)
        {
            this.Version = version;
            this.Category = category;
            this.Description = description;
            this.Tags = tags == null ? null : tags.Split(';');
        }
    }
}