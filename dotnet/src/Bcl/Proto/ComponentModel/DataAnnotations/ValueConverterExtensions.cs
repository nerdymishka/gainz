

using System;
using System.Collections.Generic;
using System.Linq;
using NerdyMishka.ComponentModel.ValueConversion;
using NerdyMishka.Reflection;
using NerdyMishka.Reflection.Extensions;

namespace NerdyMishka.ComponentModel.DataAnnotations
{
    public static class Extensions
    {

        public static IReadOnlyCollection<ValueConverter> GetValueConverters(this IProperty property, Type convertTo = null)
        {
            if(convertTo == null)
                convertTo = property.ClrType;

            var converters = property.GetMetadata<List<ValueConverter>>(
                    "ValueConverters:" + convertTo.FullName);

            if(converters != null)
                return converters;

            var attributes = property.Attributes
                    .Where(o => o is IValueConverterAttribute)
                    .ToList();

            foreach(var attr in attributes)
                ((IValueConverterAttribute)attr).Initialize();

            
            var encryption = attributes.FirstOrDefault(o => o is EncryptAttribute);
            if(encryption != null && ((EncryptAttribute)encryption).Instance == null)
            {
                var clrType = property.ClrType;
                var propertyType = property.ClrType.AsTypeInfo();
                if(propertyType.IsNullableOfT())
                    clrType = propertyType?.UnderlyingType?.ClrType ?? clrType;

                var attr = (EncryptAttribute)encryption;
                var converter = EncryptAttribute.Converters.FirstOrDefault(
                    o => o.CanConvertFrom(clrType) && o.CanConvertTo(convertTo));

                if(converter != null)
                    attr.Instance = converter;
            }

            converters = attributes
                .Cast<IValueConverterAttribute>()
                .Where(o => o.Instance != null)
                .Select(o => o.Instance)
                .ToList();


            property.SetMetadata("ValueConverters:" + convertTo.FullName, converters);
            return converters;
        }
    }
}