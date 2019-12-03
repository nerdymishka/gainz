using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.KeePass
{
    public interface IKeePassDocument
    {
       IList<DeletedObjectInfo> DeletedObjects { get; }

        IEnumerable<IKeePassGroup> Groups { get;  }

        IKeePassGroup RootGroup { get;  }

        void Add(IKeePassGroup group);

        void Remove(IKeePassGroup group);
    }
}
