using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Security.Cryptography
{
    public class CompositeKeyFragment : ICompositeKeyFragment
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
