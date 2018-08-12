using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Akashic
{
    public class RelationalColumn 
    {
        public string Name { get; set; }

        public bool IsNull { get; set; }

        public Type ClrType { get; set; }

        public string Type { get; set; }

        public bool Key { get; set; }
      

        
    }
}
