using System;
using NerdyMishka.ComponentModel.ValueConversion;

namespace NerdyMishka.ComponentModel.DataAnnotations
{
    public interface IValueConverterAttribute
    {
        Type ValueConverter { get; set; }

        ValueConverter Instance { get; set; }

        void Initialize();
    }
}