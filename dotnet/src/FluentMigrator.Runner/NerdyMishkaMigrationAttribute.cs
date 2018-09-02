using FluentMigrator;
using FluentMigrator.Infrastructure;
using FluentMigrator.Model;
using System;
using MigrationAttribute = FluentMigrator.MigrationAttribute;

namespace NerdyMishka.FluentMigrator
{
    

  

    public class NerdyMishkaMigrationAttribute : MigrationAttribute
    {
        public string Module { get; set; }

        private static long FormatDate(int year, int month, int day, int revision = 0)
        {
            return FormatDate(new DateTime(year, month, day), revision);
        }

        private static long FormatDate(DateTime version, int revision = 0) 
        {
            var str = version.ToString("yyyyMMdd");
            if(revision < 10)
                str += $"0{revision}";
            else 
                str += $"{revision}";

            return long.Parse(str);
        }

        private static long FormatDate(string version, int revision = 0)
        {
            DateTime dt = DateTime.MinValue;
            if(!DateTime.TryParse(version, out dt))
                throw new InvalidCastException($"string {version} could not be converted into a date");

            return FormatDate(dt, revision);
        }

        public NerdyMishkaMigrationAttribute(string date, int revision, 
            string module = null, 
            string description = null)
            : base(FormatDate(date, revision),  description)
        {
            this.Module = module ?? "app";
        }

        public NerdyMishkaMigrationAttribute(int year, int month, int day, int revision,
         string module = null, string description = null)
            : base(FormatDate(year, month, day, revision),  description)
        {
            this.Module = module ?? "app";
        }

        public NerdyMishkaMigrationAttribute(DateTime version, string module, string description) 
            : base(FormatDate(version), description)
        {
            this.Module = module;
        }

        public NerdyMishkaMigrationAttribute(long version, string description) 
            : base(version, description)
        {
            this.Module = "app";
        }

        public NerdyMishkaMigrationAttribute(
            long version, 
            string module = "app",
            TransactionBehavior transactionBehavior = TransactionBehavior.Default, 
            string description = null) : 
            base(version, transactionBehavior, description)

        {
            this.Module = module;
        }
    }
}