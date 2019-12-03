using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.KeePass
{
    public class KeePassAssocation : IKeePassAssociation
    {
        public string Window { get; set; }

        public string KeystrokeSequence { get; set; }
    }
}
