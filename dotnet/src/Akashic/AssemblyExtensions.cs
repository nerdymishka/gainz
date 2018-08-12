using System;
using System.Reflection;

namespace NerdyMishka.Data 
{
    public static class AssemblyExtensions 
    {

        /// <summary>
        /// Loads a <see cref="Type"/> by <paramref name="className"/> from the <paramref name="assembly"/>.
        /// </summary>
        /// <param name="assembly">The <see cref="System.Reflection.Assembly"/> reference.</param>
        /// <param name="className">The name of the class to retrieve.</param>
        /// <returns>The <see cref="Type"/> information.</returns>
        public static Type LoadClass(this Assembly assembly, string className)
        {
#if NETSTANDARD1_3
            return assembly.GetType(className, true, true);
#else
            return assembly.GetType(className);
#endif
        }

        /// <summary>
        /// Loads the <see cref="System.Reflection.FieldInfo"/> by the class and field name.
        /// </summary>
        /// <param name="assembly">The <see cref="System.Reflection.Assembly"/> reference.</param>
        /// <param name="classAndFieldName">The name of the class and the name of the field.</param>
        /// <returns>The <see cref="System.Reflection.FieldInfo"/>.</returns>
        public static FieldInfo LoadField(this Assembly assembly, string classAndFieldName)
        {
            var index = classAndFieldName.LastIndexOf(".");
            var className = classAndFieldName.Substring(0, index);
            var fieldName = classAndFieldName.Substring(index);

            return assembly.LoadField(className, fieldName);
        }

        /// <summary>
        /// Loads the <see cref="System.Reflection.FieldInfo"/> by the class and field name.
        /// </summary>
        /// <param name="assembly">The <see cref="System.Reflection.Assembly"/> reference.</param>
        /// <param name="className">The name of the class.</param>
        /// <param name="fieldName">The name of the field</param>
        /// <returns>The <see cref="System.Reflection.FieldInfo"/>.</returns>
        public static FieldInfo LoadField(this Assembly assembly, string className, string fieldName)
        {
            var type = assembly.LoadClass(className);
            return type.GetRuntimeField(fieldName);
        }
    }
}