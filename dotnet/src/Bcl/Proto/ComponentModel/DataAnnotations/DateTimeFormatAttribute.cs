using System;
using NerdyMishka.ComponentModel.ValueConversion;

namespace NerdyMishka.ComponentModel.DataAnnotations
{
    [System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public sealed class DateTimeFormatAttribute : ValueConverterAttribute
    {
        public string Format { get; set; }

        public string Provider { get; set; }

        public bool IsUtc { get; set; } = true;

        public string Name { get; set; } = "Default";

        public DateTimeFormatAttribute(Type valueConverter)
            :base(valueConverter)
        {
            
        }

        public DateTimeFormatAttribute()
            :base(typeof(DefaultDateTimeToStringConverter))
        {
          
        }

        public DateTimeFormatAttribute(string format, string provider)
            :base(typeof(DefaultDateTimeToStringConverter))
        {
            this.Format = format;
            this.Provider = provider;
           
        }

        public DateTimeFormatAttribute(string format)
            :base(typeof(DefaultDateTimeToStringConverter))
        {
            this.Format = format;
        }

        public class DefaultDateTimeToStringConverter : ValueConverter
        {
            public string Format { get; set; }

            public bool IsUtc { get; set; } = true;

            public override Type FromType => typeof(DateTime);

            public override Type ToType => typeof(string);

            public override bool CanConvertFrom(Type type)
            {
                return type == typeof(DateTime);
            }

            public override bool CanConvertTo(Type type)
            {
                return type == typeof(string);
            }

            public override object ConvertFrom(object value)
            {
                if(value == null)
                    return null;

                
                DateTime x = (DateTime)value;
                if(x.Kind == DateTimeKind.Local)
                   x = x.ToUniversalTime();

                return x.ToString(this.Format);
            }

            public override object ConvertTo(object value)
            {
                if(value == null)
                    return null;

                string v = (string)value;

                if(DateTime.TryParse(v, out DateTime result))
                    return result;

                return null;
            }
        }
    }
}