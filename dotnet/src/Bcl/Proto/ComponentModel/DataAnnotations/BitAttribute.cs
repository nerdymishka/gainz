using System;
using System.Linq;
using NerdyMishka.ComponentModel.ValueConversion;

namespace NerdyMishka.ComponentModel.DataAnnotations
{

    [System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public sealed class BitAttribute : ValueConverterAttribute
    {
        public bool MatchCase { get; set; } = false;

        public string[] Yes { get; set; }

        public string[] No { get; set; }

        // This is a positional argument

        // This is a positional argument
        public BitAttribute(Type valueConverter) : base(valueConverter)
        {

            
        }

        public BitAttribute(string yes = null, string no = null) : base(typeof(BooleanToStringConverter))
        {
            yes = yes ?? "true";
            no = no ?? "false";
            
            this.Yes = yes.Split('|');
            this.No = no.Split('|');
        }


        public class BooleanToStringConverter : ValueConverter
        {
            public string[] Yes { get; set; }

            public string[] No { get; set; }

            public bool MatchCase { get; set; }

            public override Type FromType => typeof(Boolean);

            public override Type ToType => typeof(string);

            public override bool CanConvertFrom(Type type)
            {
                return type == typeof(Boolean);
            }

            public override bool CanConvertTo(Type type)
            {
                return type == typeof(string);
            }

         


            public override object ConvertFrom(object value)
            {
               if(value == null)
                    return null;
                

                string v = (string)value;

                if(this.MatchCase)
                {
                    return this.Yes.Any(o => o == v);
                }

                return this.Yes.Any(o => o.ToLowerInvariant() == v);
            }

            public override object ConvertTo(object value)
            {
                if(value == null)
                    return null;

                
                bool x = (bool)value;
                return x == true ? this.Yes[0] : this.No[0];
            }
        }
    }
}