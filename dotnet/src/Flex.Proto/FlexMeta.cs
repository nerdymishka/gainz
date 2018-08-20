using System;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Flex
{
    public class FlexMeta : FObject
    {
        private FlexAttributeList attributes;

       
        public FlexAttributeList Attributes
        {
            get
            {
                if (this.attributes == null)
                    this.attributes = new FlexAttributeList();

                return this.attributes;
            }
        }
    }
}
