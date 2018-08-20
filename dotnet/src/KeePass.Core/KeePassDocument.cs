using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.KeePass
{
    public class KeePassDocument : IKeePassDocument
    {
        private List<IKeePassGroup> groups = new List<IKeePassGroup>();
        private KeePassPackage owner = null;


        public KeePassDocument(KeePassPackage package)
        {

            this.owner = package;
            this.DeletedObjects = new List<DeletedObjectInfo>();
        }

        public IList<DeletedObjectInfo> DeletedObjects { get; protected set; }

        public IKeePassGroup RootGroup { get; protected set; }

        public IEnumerable<IKeePassGroup> Groups => this.groups;

        public void Add(IKeePassGroup group)
        {
            var one = this.groups.SingleOrDefault(o => o.Uuid.EqualTo(group.Uuid));
            if(one == null)
            {
                if (this.RootGroup == null)
                    this.RootGroup = group;

                this.groups.Add(group);

                if (group is KeePassGroup)
                    ((KeePassGroup)group).Owner = this.owner;
            }
        }

        public void Remove(IKeePassGroup group)
        {
            if (this.groups.Remove(group))
            {
                if (group is KeePassGroup)
                    ((KeePassGroup)group).Owner = null;

                if (this.RootGroup == group)
                {
                    if (this.groups.Count > 0)
                        this.RootGroup = this.groups[0];
                    else
                        this.RootGroup = null;
                }
            }
        }
    }
}
