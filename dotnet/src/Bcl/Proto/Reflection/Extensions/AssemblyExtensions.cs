using System;
using System.Reflection;

namespace NerdyMishka.Reflection.Extensions
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
            var path = GetPath(assembly);
            if(string.IsNullOrWhiteSpace(path))
                return null;

            return System.IO.Path.GetDirectoryName(assembly.GetPath());
        }
    }
}