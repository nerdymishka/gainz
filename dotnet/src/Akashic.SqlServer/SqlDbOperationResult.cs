using System;
using NerdyMishka.Data;

namespace NerdyMishka.Data 
{

    public class SqlDbOperationResult : IDbOperationResult
    {
        public bool Ok { get; set; }
        public bool IsSupported { get; set; }
        public Exception Exception { get; set; }
    }
}