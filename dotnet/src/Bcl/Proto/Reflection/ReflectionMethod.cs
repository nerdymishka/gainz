
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace NerdyMishka.Reflection
{

    public class ReflectionMethod : ReflectionMember, IMethod
    {
        

        private List<IParameter> parameters;

        private Delegate method;

        private Type[] generateTypeArguments = Array.Empty<Type>();

        private IParameter returnParameter;
        public MethodInfo MethodInfo { get; protected set; }

        public ReflectionMethod(MethodInfo method, IReflectionFactory factory)
        {
            this.Factory = factory;
           
            this.Initialize(method);   
        }

        public ReflectionMethod(MethodInfo info, ParameterInfo[] parameters, IReflectionFactory factory)
        {
            this.Factory = factory;
           
             this.MethodInfo = info;
            this.Name = info.Name;
            this.parameters = new List<IParameter>();
            if(info.IsGenericMethodDefinition)
                this.generateTypeArguments = info.GetGenericArguments();

            

            if(info.ReturnParameter != null)
                this.returnParameter = new ReflectionParameter(info.ReturnParameter); 
        }


        protected virtual void Initialize(MethodInfo info)
        {
            this.MethodInfo = info;
            this.Name = info.Name;
            this.parameters = new List<IParameter>();
            if(info.IsGenericMethodDefinition)
                this.generateTypeArguments = info.GetGenericArguments();

            foreach(var param in info.GetParameters().OrderBy(o => o.Position))
            {
                var item = new ReflectionParameter(param);
                this.parameters.Add(new ReflectionParameter(param));
            }

            if(info.ReturnParameter != null)
                this.returnParameter = new ReflectionParameter(info.ReturnParameter);
        }

        public bool IsStatic => this.MethodInfo.IsStatic;

        public bool IsPublic => this.MethodInfo.IsPublic;

        public bool IsPrivate => this.MethodInfo.IsPrivate;

        public bool IsInstance => !this.MethodInfo.IsStatic;

        public bool IsVirtual => this.MethodInfo.IsVirtual;

        public bool IsFamily => this.MethodInfo.IsFamily;

        public bool IsAssembly => this.MethodInfo.IsAssembly;

        public bool IsGenericMethodDefinition => this.IsGenericMethodDefinition;

        public IReadOnlyCollection<Type> GenericArguments
        {
            get {
                if(this.generateTypeArguments == null)
                    this.generateTypeArguments = this.MethodInfo.GetGenericArguments();

                return this.generateTypeArguments;
            }
        }

        public virtual IReadOnlyCollection<IParameter> Parameters
        {
            get{
                if(this.parameters == null)
                {
                    this.parameters = new List<IParameter>();
                    foreach(var param in this.MethodInfo.GetParameters().OrderBy(o => o.Position))
                    {
                        this.parameters.Add(this.Factory.CreateParameter(param));
                    }
                }
                return this.parameters;
            }
        }

        public virtual IReadOnlyCollection<Type> ParameterTypes => this.Parameters
            .Select(o => o.ParameterInfo.ParameterType)
            .ToList();

        public virtual IParameter ReturnParameter => this.returnParameter;


        public T Invoke<T>(object instance, params object[] parameters)
        {
            return (T)this.Invoke(instance, parameters);
        }

        public object Invoke(object instance, params object[] parameters)
        {
            // based upon code from stack overflow. 
            // https://stackoverflow.com/questions/10313979/methodinfo-invoke-performance-issue
            if(method != null)
            {
                return method.DynamicInvoke(instance, parameters);
            }
            
            var instanceExpression = Expression.Parameter(typeof(object), "obj");
            var argumentsExpression = Expression.Parameter(typeof(object[]), "arguments");
            var argumentExpressions = new List<Expression>();
        
            foreach (var parameter in this.Parameters)
            {
                var parameterInfo = parameter.ParameterInfo;
                argumentExpressions.Add(
                    Expression.Convert(
                        Expression.ArrayIndex(argumentsExpression, 
                        Expression.Constant(parameter.Position)), 
                        parameterInfo.ParameterType));
            }

            UnaryExpression unary = null;
            if(this.IsStatic)
            {
                unary = Expression.Convert(instanceExpression, this.MethodInfo.ReflectedType);
            }

            var callExpression = Expression.Call(
                unary, this.MethodInfo, argumentExpressions);

    
            if (callExpression.Type == typeof(void))
            {
                var voidDelegate = Expression.Lambda<Action<object, object[]>>(
                    callExpression, instanceExpression, argumentsExpression)
                    .Compile();

                Func<object, object[], object> action = (obj, arguments) => { 
                    voidDelegate(obj, arguments); 
                    return null;
                };

                this.method = action;

            }
            else 
            {
                this.method = Expression.Lambda<Func<object, object[], object>>(
                    Expression.Convert(callExpression, 
                        typeof(object)), 
                        instanceExpression, argumentsExpression).Compile();
            }
            
            
            return this.method.DynamicInvoke(instance, parameters);
        }


        public override IReflectionMember LoadAttributes(bool inherit = true)
        {
            this.SetAttibutes(
                CustomAttributeExtensions.GetCustomAttributes(this.MethodInfo, inherit));

            return this;
        }
    }
}