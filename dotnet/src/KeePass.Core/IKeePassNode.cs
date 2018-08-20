using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.KeePass
{
    public interface IKeePassNode
    {
        byte[] Uuid { get; set; }

        int IconId { get; set; }

        IKeePassAuditFields AuditFields { get; }

        string Name { get; set; }

        string Notes { get; set; }

        IKeePassPackage Owner { get; }
    }
}
