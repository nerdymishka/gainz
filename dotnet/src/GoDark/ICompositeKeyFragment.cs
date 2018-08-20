using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Security.Cryptography
{
    public interface ICompositeKeyFragment
    {
        
        byte[] UnprotectAndCopyData();
    
    }
}
