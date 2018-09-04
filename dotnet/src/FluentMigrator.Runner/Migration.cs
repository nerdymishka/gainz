using System;
using Microsoft.Extensions.DependencyInjection;
using MigrationBase = FluentMigrator.Migration;

namespace NerdyMishka.FluentMigrator
{
    public interface IMigrationWithServiceProvider
    {
        System.IServiceProvider ServiceProvider { get; set; }

        string DefaultSchemaName { get; set; }

        string ProviderName {get; set;}
    }

    public abstract class Migration : MigrationBase, IMigrationWithServiceProvider
    {
        public IServiceProvider ServiceProvider { get; set; }

        public string ProviderName {get; set; }

        public string DefaultSchemaName {get; set; }
    }
}