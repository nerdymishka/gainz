using System;
using System.ComponentModel;
using Castle.DynamicProxy;

namespace NerdyMishka.ComponentModel.ChangeTracking
{
    /*
    public class PropertyChangeIntercepter : IInterceptor
    {
        private PropertyChangedEventHandler handler;

        public void Intercept(IInvocation invocation)
        {
            var type = NerdyMishka.Reflection.ReflectionCache.GetOrAdd(invocation.TargetType);



            if(invocation?.Method != null)
            {
                

                var isSet = invocation.Method.Name.StartsWith("set_");
                var isget = invocation.Method.Name.StartsWith("get_");

                if(isSet)
                {
                    var propertyName = invocation.Method.Name.Substring(4);
                    var propertyInfo = type.Properties[propertyName].PropertyInfo;
                    var previousValue = propertyInfo.GetValue(invocation.Proxy, null);
                    invocation.Proceed();
                    var currentValue = propertyInfo.GetValue(invocation.Proxy, null);
                    
                    if(!previousValue.Equals(currentValue))
                    {

                    }
                }

                switch(invocation?.Method.Name)
                {
                    case "add_PropertyChanged":
                        this.handler += (PropertyChangedEventHandler)invocation.Arguments[0];
                        break;

                    case "remove_PropertyChanged":
                        this.handler -= (PropertyChangedEventHandler)invocation.Arguments[0];
                        break;

                    default:
                        invocation.Proceed();
                        break;
                }
            }

            
           

           
        }
    }*/
}