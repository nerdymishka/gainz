﻿using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Flex
{

    public class FlexDateTimeOptions
    {
        public string Format { get; set; }

        public string Provider { get; set; }

        public bool IsUtc { get; set; }

        public string Name { get; set; }
    }

    [System.AttributeUsage(AttributeTargets.Property, Inherited = true, AllowMultiple = true)]
    public sealed class DateTimeFormatAttribute : Attribute
    {
        public string Format { get; set; }

        public string Provider { get; set; }

        public bool IsUtc { get; set; } = true;

        public string Name {get; set; } = "Default";

        public DateTimeFormatAttribute()
        {
            
        }

        public DateTimeFormatAttribute(string format, string provider)
        {
            this.Format = format;
            this.Provider = provider;
        }

        public DateTimeFormatAttribute(string format)
        {
            this.Format = format;
        }
    }
}