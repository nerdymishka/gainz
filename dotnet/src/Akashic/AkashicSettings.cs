using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace NerdyMishka.Data
{
    public class AkashicSettings
    {
        static AkashicSettings()
        {
            MapData = (dr, type) =>
            {
                var instance = Activator.CreateInstance(type);
                var properties = type.GetRuntimeProperties();

                int i = 0;
                while(i < dr.FieldCount)
                {
                    var name = dr.GetName(i).ToLower();
                    var property = properties.SingleOrDefault(o => o.Name.ToLower() == name);
                    var value = dr.GetValue(i);

                    if (value == DBNull.Value)
                        value = null;

                    var typeInfo = property.PropertyType.GetTypeInfo();
                    if (typeInfo.IsGenericType && 
                         typeInfo.GetGenericTypeDefinition() == typeof(Nullable<>))
                    {
                        // for instances where propertype may be int32
                        // and the value is int64
                        if(value != null)
                        {
                            var propertyType = property.PropertyType.GetGenericArguments()[0];
                            value = Convert.ChangeType(value, propertyType);
                        }
                     
                        var constant = Expression.Constant(value, property.PropertyType);
                        value = constant.Value;
                    }
                    else if (value != null && property.PropertyType != value.GetType())
                    {
                        value = Convert.ChangeType(value, property.PropertyType);
                    }

                    property.SetValue(instance, value);
                    i++;
                }
                

                return instance;
            };

            ParameterPrefix = ':';
            AdoParameterPrefix = '@';
        }

        public static Func<IDataReader, Type, object> MapData { get; set; }

        public static char ParameterPrefix { get; set; }

        public static char AdoParameterPrefix { get; set; }
    }
}
