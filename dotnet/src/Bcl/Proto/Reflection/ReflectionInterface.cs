

using System;
using System.Collections.Generic;

namespace NerdyMishka.Reflection
{
    public class ReflectionInterface : ReflectionType, IInterface
    {
      

        public ReflectionInterface(Type type, IReflectionFactory factory) :base(type, factory)
        {
            this.ClrType = type;
            this.Name = type.Name;
        }

       

        
    }
}