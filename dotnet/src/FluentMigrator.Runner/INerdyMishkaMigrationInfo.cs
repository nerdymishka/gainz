using System;
using FluentMigrator;
using FluentMigrator.Infrastructure;

namespace NerdyMishka.FluentMigrator.Runner
{

    public interface INerdyMishkaMigrationInfo : IMigrationInfo
    {
        string Module { get; }
    }

    public class NerdyMishkaMigrationInfo : MigrationInfo, INerdyMishkaMigrationInfo
    {
        public NerdyMishkaMigrationInfo(
            long version, 
            string module = null,
            string description = null, 
            TransactionBehavior transactionBehavior = TransactionBehavior.Default, 
            bool isBreakingChange = false, 
            Func<IMigration> migrationFunc = null) 
            : base(version, description, transactionBehavior, isBreakingChange, migrationFunc)
        {
            this.Module = module;
        }

        public string Module  { get; set; }
    }
}