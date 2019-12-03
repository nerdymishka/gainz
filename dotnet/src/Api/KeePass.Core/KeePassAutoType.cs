using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.KeePass
{
    public class KeePassAutoType : IKeePassAutoType
    {
        private IKeePassAssociation association;

        public bool Enabled { get; set; }

        public int DataTransferObfuscation { get; set; }

        public IKeePassAssociation Association
        {
            get { if (this.association == null)
                    this.association = new KeePassAssocation();
                return this.association;
            }
        }
    }
}
