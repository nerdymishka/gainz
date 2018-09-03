using System;

namespace Nexus.Data
{
    [Flags]
    public enum ResourceActions: byte
    {
        None = 0,
        List = 1,
        Read = 2,
        Write = 4,
        Delete = 8,
        Execute = 16
    }    
}