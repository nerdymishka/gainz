using System;
using FluentMigrator.Infrastructure;

namespace NerdyMishka.FluentMigrator.Runner
{

    internal static class Extensions
    {

        public static bool IsAttributed(this IMigrationInfo migrationInfo)
        {
            return !(migrationInfo is NonAttributedMigrationToMigrationInfoAdapter);
        }

        
        public static bool IsInNamespace(
            this Type type,
            string @namespace,
            bool loadNestedNamespaces)
        {
            if (string.IsNullOrEmpty(@namespace))
                return true;

            if (type.Namespace == null)
                return false;

            if (type.Namespace == @namespace)
                return true;

            if (!loadNestedNamespaces)
                return false;

            var matchNested = @namespace + ".";
            return type.Namespace.StartsWith(matchNested, StringComparison.Ordinal);
        }
    }
}