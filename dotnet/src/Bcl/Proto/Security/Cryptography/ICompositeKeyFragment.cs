

using System;

namespace NerdyMishka.Security
{
    public interface ICompositeKeyFragment 
    {
        
        ReadOnlySpan<byte> CopyData();
    
    }
}

