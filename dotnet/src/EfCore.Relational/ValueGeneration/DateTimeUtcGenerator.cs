using System;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace NerdyMishka.EfCore.ValueGeneration
{
    public class DateTimeUtcGenerator : ValueGenerator<DateTime>
    {
        public override bool GeneratesTemporaryValues => true;

        public override DateTime Next(EntityEntry entry)
        {
            return DateTime.UtcNow;
        }
    }
}