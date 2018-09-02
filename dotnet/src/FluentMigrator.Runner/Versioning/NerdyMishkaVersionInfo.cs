using System.Collections.Generic;
using FluentMigrator.Runner.Versioning;
using System;
using System.Linq;

namespace NerdyMishka.FluentMigrator.Runner.Versioning
{
    public interface INerdyMishkaVersionInfo : IVersionInfo
    {
        void AddAppliedMigration(string module, long version);

        IEnumerable<Tuple<string, long>> AppliedModuleMigrations { get; }

        bool HasAppliedMigration(string module, long version);

        long Latest(string module);
    }


    public class NerdyMishkaVersionInfo : INerdyMishkaVersionInfo
    {
        private List<Tuple<string, long>> appliedMigrations = new List<Tuple<string, long>>();
        private List<long> appliedAppMigrations = new List<long>();

        public IEnumerable<Tuple<string, long>> AppliedModuleMigrations => this.appliedMigrations;

        public void AddAppliedMigration(string module, long version)
        {
            if(string.IsNullOrWhiteSpace(module))
                module = "app";

            this.appliedMigrations.Add(new Tuple<string, long>(module, version));

            if(module == "app")
                this.appliedAppMigrations.Add(version);
        }

        public void AddAppliedMigration(long version)
        {
            this.AddAppliedMigration("app", version);
        }

        public IEnumerable<long> AppliedMigrations() => this.appliedAppMigrations;

        public bool HasAppliedMigration(string module, long version)
        {
            var item = new Tuple<string, long>(module, version);
            return this.appliedMigrations.Contains(item);
        }

        public bool HasAppliedMigration(long version)
        {
            return this.appliedAppMigrations.Contains(version);
        }

        public long Latest(string module)
        {
            if(module == null)
            {
                var next = this.appliedMigrations
                            .OrderByDescending(o => o.Item2)
                            .FirstOrDefault();

                if(next == null)
                    return 0L;

                return next.Item2;
            }

            var version = this.appliedMigrations.Where(o => o.Item1 == module)
                        .OrderByDescending(o => o.Item2).FirstOrDefault();

            return version.Item2;
        }

        public long Latest()
        {
            return this.appliedAppMigrations
                .OrderByDescending(o => o)
                .FirstOrDefault();
        }
    }
}
