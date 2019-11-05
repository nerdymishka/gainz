using System;
using System.Reflection;

namespace NerdyMishka.Reflection
{
    internal static class AssemblyExtensions
    {
        public static string GetPath(this Assembly assembly)
        {
            if(!string.IsNullOrWhiteSpace(assembly.CodeBase))
            {
                return assembly.CodeBase.Replace("file:///", "");
            }

            return null;
        }

        public static string GetDirectoryName(this Assembly assembly)
        {
            return System.IO.Path.GetDirectoryName(assembly.GetPath());
        }
    }
}