

using System;
using NerdyMishka.ComponentModel.DataAnnotations;

namespace Tests 
{
    public class SimpleConfigurationAttributes
    {
        [Bit("enabled", "disabled")]
        public bool IsEnabled { get; set; } = true;

        [Encrypt]
        public string ConnectionString { get; set; } = "Server=localhost;";

        [Hash]
        public string Password { get; set; } = "my-great-and-terrible-pw";
    }

    public class SimpleConfigurationValues
    {
        public string String { get; set; } = "test-value";

        public int Int32 { get; set; } = 443;

        public int? NullableInt32 { get; set; } = null;

        public long Int64 { get; set; } = long.MaxValue;

        public long? NullableInt64 { get; set; } = long.MaxValue;

        public DateTime DateTime { get; set; } = DateTime.UtcNow;

        public Double Double { get; set; } = 80923.2342;

        public Decimal Decimal { get; set; } = 3421231234.23423M;

        public byte[] Bytes { get; set; } = System.Text.Encoding.UTF8.GetBytes("bytes");

        public char[] Chars { get; set; } = "chars".ToCharArray();

        public bool Boolean { get; set; } = true;
    }
    
}