using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NerdyMishka.Flex
{
    public abstract class FlexEnumerable : FlexObject,
        IEnumerable<FObject>
    {
        public virtual long Count => 0;

    
        protected virtual IEnumerator<FObject> GetFObjectEnumerator()
        {
            return Enumerable.Empty<FObject>().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetFObjectEnumerator();
        }

        IEnumerator<FObject> IEnumerable<FObject>.GetEnumerator()
        {
            return this.GetFObjectEnumerator();
        }
    }
}
