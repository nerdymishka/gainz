using NerdyMishka.Security.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.KeePass
{
    /// <summary>
    /// A master key fragment is a piece of master key. Think of it
    /// as a single piece of the Tri-Force that must be collected
    /// into a composite artifact before it can truly grant 
    /// the power you seek. e.g. opening the KeePass file. 
    /// </summary>
    public class MasterKeyFragment : IMasterKeyFragment
    {
        private ProtectedMemoryBinary binary;

        public byte[] UnprotectAndCopyData()
        {
            return this.binary.UnprotectAsBytes();
        }


        protected void SetData(byte[] data)
        { 
            this.binary = new ProtectedMemoryBinary(data, true);
        }
    }
}
