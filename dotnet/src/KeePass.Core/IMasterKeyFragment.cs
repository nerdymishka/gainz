using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.KeePass
{
    public interface IMasterKeyFragment
    {
        byte[] UnprotectAndCopyData();
    }
}
