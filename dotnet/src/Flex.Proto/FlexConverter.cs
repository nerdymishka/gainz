using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace NerdyMishka.Flex
{
    public class FlexConverter
    {

        public static FObject ToFObject(object value)
        {
            switch (value)
            {
                case null:
                    return new FlexPrimitive(null);

                case FObject p:
                    return p;


                case IDictionary dictionary:
                    var flexObject = new FlexObject();
                  

                    return flexObject;
                 

                case IList list:
                    return null;

                default:
                    return new FlexPrimitive(value);
            }
        }
    }
}
