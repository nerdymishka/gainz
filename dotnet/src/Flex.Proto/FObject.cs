using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NerdyMishka.Flex
{
    public class FObject
    {
        public virtual FlexType FlexType => FlexType.FObject;

        public virtual object Unbox()
        {
            return null;
        }


        public override string ToString()
        {
            return string.Empty;
        }

    }
}
