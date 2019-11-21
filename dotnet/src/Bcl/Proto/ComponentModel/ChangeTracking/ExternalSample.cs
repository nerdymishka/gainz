
using System.Collections.Generic;

namespace NerdyMishka.ComponentModel.ChangeTracking
{
    
    public class ExternalSample
    {
        public Child1 Child { get; set; }

        public List<Child2> Children { get; set; }
    }

    public class Child1
    {
        public int Id { get; set; }

        public string Name  {get; set; }

        public byte[] Value  { get; set; }
    }

    public class Child2
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }
    }
}