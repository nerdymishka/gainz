

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace NerdyMishka.Reflection
{
    public class ReflectionType : ReflectionMember, IType, IUnderlyingType, IItemType
    {
        private List<IMethod> methods;
        private List<IProperty> properties;

        private bool allMethodsLoaded = false;
        private TypeInfo info;

        private IndexedReadOnlyCollection<IInterface> interfaces;

        public ReflectionType(TypeInfo info, IReflectionFactory factory) 
        : this(info.AsType(), factory)
        {

        }

        public ReflectionType(Type type, IReflectionFactory factory)
        {
            this.ClrType = type;
            this.Name = type.Name;
            this.IsDataType = type.IsPrimitive || ReflectionBuilder.DataTypes.Any(o => o == type);
            this.Factory = factory;
        }

        public IReadOnlyCollection<IInterface> Interfaces 
        {
            get{
                if(this.interfaces == null)
                {
                    this.interfaces = new IndexedReadOnlyCollection<IInterface>();
                    var interfaces = this.ClrType.GetInterfaces();
                    foreach(var contract in interfaces)
                        this.interfaces.Add(contract.FullName, this.Factory.CreateInterface(contract));
                }

                return this.interfaces;
            }
        }

        public IType BaseType 
        {
            get{
                if(this.ClrType.BaseType == null)
                    return null;

                return ReflectionCache.FindOrAdd(this.ClrType.BaseType);
            }
        }

        public IType UnderlyingType { get; set; }

        public IType ItemType { get; set; }

        public virtual bool IsDataType { get; protected set; }

        public string Namespace => this.ClrType.Namespace;

        public string FullName => this.ClrType.FullName;

        public virtual IReadOnlyCollection<IProperty> Properties 
        {
            get {
                if(this.properties == null)
                {
                    this.LoadProperties(false);
                }

                return this.properties;
            }
        }

        public virtual IReadOnlyCollection<IMethod> Methods
        {
            get{
                if(this.methods == null)
                {
                    this.LoadMethods(false);
                }

                return this.methods;
            }
        }

        public IType LoadProperties(bool inherit = false)
        {
            if(inherit)
            {
                 return this.LoadProperties(
                    BindingFlags.Public | 
                    BindingFlags.NonPublic | 
                    BindingFlags.Static | 
                    BindingFlags.Instance | 
                    BindingFlags.Default |
                    BindingFlags.FlattenHierarchy);
            }

            return this.LoadProperties(
               BindingFlags.Public | 
                    BindingFlags.NonPublic | 
                    BindingFlags.Static | 
                    BindingFlags.Instance | 
                    BindingFlags.Default |
                    BindingFlags.DeclaredOnly |
                    BindingFlags.FlattenHierarchy);
        }

        public IType LoadMethods(bool inherit = false)
        {
            if(inherit)
            {
                return this.LoadMethods(
                    BindingFlags.Public | 
                    BindingFlags.NonPublic | 
                    BindingFlags.Static | 
                    BindingFlags.Instance | 
                    BindingFlags.Default |
                    BindingFlags.FlattenHierarchy);

            } else {
               return this.LoadMethods(
                   BindingFlags.Public | 
                    BindingFlags.NonPublic | 
                    BindingFlags.Static | 
                    BindingFlags.Instance | 
                    BindingFlags.Default |
                    BindingFlags.DeclaredOnly |
                    BindingFlags.FlattenHierarchy
               );
            }
        }

        public IType LoadMethods(BindingFlags flags)
        {
            this.methods = new List<IMethod>();
            MethodInfo [] methodInfos = null;
            methodInfos = this.ClrType.GetMethods(flags);
            foreach(var mi in methodInfos)
                this.methods.Add(this.Factory.CreateMethod(mi));

            return this;
        }

        public IType LoadProperties(BindingFlags flags)
        {
            this.properties = new List<IProperty>();
            PropertyInfo [] propertyInfos = this.ClrType.GetProperties(flags);
            foreach(var pi in propertyInfos)
                this.properties.Add(this.Factory.CreateProperty(pi, this));

            return this;
        }

        public IProperty GetProperty(
            string name, 
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
            return this.GetProperties(name, flags)
                .FirstOrDefault();
        }


        public IEnumerable<IProperty> GetProperties(
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
            bool isStatic = flags.HasFlag(BindingFlags.Static);
            bool isInstance = flags.HasFlag(BindingFlags.Instance);
            bool isPublic = flags.HasFlag(BindingFlags.Public);
            bool isPrivate = flags.HasFlag(BindingFlags.NonPublic);
  

            var query = this.Properties.AsQueryable();

            if(isStatic != isInstance)
            {
                if(isStatic)
                    query = query.Where(o => o.IsStatic);
                else 
                    query = query.Where(o => o.IsInstance);
            }

            if(isPublic != isPrivate)
            {
                if(isPublic)
                    query = query.Where(o => o.IsPublic);
                else 
                    query = query.Where(o => o.IsPrivate);
            }
                
            return query;
        }

        public IEnumerable<IProperty> GetProperties(
            string name, 
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance)
        {
           
            bool isCaseInsenstive = flags.HasFlag(BindingFlags.IgnoreCase);

            var query = this.GetProperties();

            if(isCaseInsenstive)
            {
                var normalized = name.ToLowerInvariant();
                query = query.Where(o => o.Name.ToLowerInvariant() == name);
            } else {
                query = query.Where(o => o.Name == name);
            }
                
            return query;
        }


        public IMethod GetMethod(string name, Type[] pararmeterTypes = null)
        {
            return this.GetMethods(name, parameterTypes: pararmeterTypes)
                .FirstOrDefault();
        }

        public IMethod GetMethod(string name, 
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance, 
            Type[] genericArgTypes = null, Type[] parameterTypes = null)
        {
            return this.GetMethods(name, flags, genericArgTypes, parameterTypes)
                .FirstOrDefault();
        }

        public IEnumerable<IMethod> GetMethods(
            string name, 
            BindingFlags flags = BindingFlags.Public | BindingFlags.Instance, 
            Type[] genericArgTypes = null, Type[] parameterTypes = null)
        {
            bool isStatic = flags.HasFlag(BindingFlags.Static);
            bool isInstance = flags.HasFlag(BindingFlags.Instance);
            bool isPublic = flags.HasFlag(BindingFlags.Public);
            bool isPrivate = flags.HasFlag(BindingFlags.NonPublic);
            bool isCaseInsenstive = flags.HasFlag(BindingFlags.IgnoreCase);

            var query = this.Methods.AsQueryable();

            if(isStatic != isInstance)
            {
                if(isStatic)
                    query = query.Where(o => o.IsStatic);
                else 
                    query = query.Where(o => o.IsInstance);
            }

            if(isPublic != isPrivate)
            {
                if(isPublic)
                    query = query.Where(o => o.IsPublic);
                else 
                    query = query.Where(o => o.IsPrivate);
            }

            if(isCaseInsenstive)
            {
                var normalized = name.ToLowerInvariant();
                query = query.Where(o => o.Name.ToLowerInvariant() == name);
            } else {
                query = query.Where(o => o.Name == name);
            }

            if(parameterTypes == null)
            {
                query = query.Where(o => o.Parameters.Count == 0);
            } else {
                query = query.Where(o => o.Parameters.Count != 0 &&
                    o.ParameterTypes.EqualTo(parameterTypes));
            }

            if(genericArgTypes == null)
            {
                query = query.Where(o => o.MethodInfo.IsGenericMethodDefinition == false);
            } else {
                query = query.Where(o => o.GenericArguments.EqualTo(genericArgTypes));
            }
                
            return query;
        }



    }
}