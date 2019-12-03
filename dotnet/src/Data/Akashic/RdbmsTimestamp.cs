using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Data
{
    public struct RdbmsTimestamp
    {
       
        public Int64 Value { get; set; }

        public static implicit operator RdbmsTimestamp(long data)
        {
            return new RdbmsTimestamp() { Value = data };
        }

        public static implicit operator RdbmsTimestamp(int data)
        {
            return new RdbmsTimestamp() { Value = data };
        }

        public static implicit operator RdbmsTimestamp(byte[] data)
        {
            return new RdbmsTimestamp() { Value = data.ToInt64() };
        }

        public static implicit operator long(RdbmsTimestamp data)
        {
            return data.Value;
        }

        public static implicit operator byte[](RdbmsTimestamp data)
        {
            return data.Value.ToBytes();
        }
    };
}
