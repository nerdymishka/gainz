using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.KeePass
{
    public class MasterKey : IEnumerable<IMasterKeyFragment>
    {
        private List<IMasterKeyFragment> fragments = new List<IMasterKeyFragment>();


        public MasterKey()
        {

        }

        

        public void Add(IMasterKeyFragment fragment)
        {
            this.fragments.Add(fragment);
        }

        public void Clear()
        {
            this.fragments.Clear();
        }

        public IEnumerator<IMasterKeyFragment> GetEnumerator()
        {
            return this.fragments.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        
    }
}
