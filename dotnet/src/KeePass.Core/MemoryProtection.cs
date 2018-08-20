using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.KeePass
{
    
    /// <summary>
    /// 
    /// </summary>
    public class MemoryProtection
    {
        public MemoryProtection()
        {
            this.ProtectPassword = true;
        }

        public bool ProtectedTitle { get; set; }

        public bool ProtectUserName { get; set; }

        public bool ProtectPassword { get; set; }

        public bool ProtectUrl { get; set; }

        public bool ProtectNotes { get; set; }
    }
}
