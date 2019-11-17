using System;

namespace NerdyMishka.ComponentModel.DataAnnotations

{

    [System.AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class EncryptAttribute : ValueConverterAttribute
    {
        public string Encoding { get; set; }

        public EncryptAttribute(Type valueConverter = null, string encoding = null) :base(valueConverter)
        {
            this.Encoding = encoding ?? "UTF-8";
           
        }
    }

}


   