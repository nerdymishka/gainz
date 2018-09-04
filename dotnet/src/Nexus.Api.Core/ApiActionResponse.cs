
using System;
using System.Collections.Generic;

namespace Nexus.Api
{
    public class ApiActionResponse<T>
    {
        public bool Ok { get; set; }
        
        public T Result { get; set; }

        public string[] ErrorMessages { get; set; }

    }
}