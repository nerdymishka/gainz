

using System;
using System.Collections.Generic;

namespace NerdyMishka.Reflection
{
    public class GroupEntry<T> where T: class, new()
    {
        public GroupEntry() 
        {

        }

        private List<T> entries = new List<T>();

        public string Name { get; internal protected set; }

        public string FullName { get; internal protected set;}

        public Type Type  { get; internal protected set; }

        public bool IsInherited { get; internal protected set; }

        public IReadOnlyCollection<T> Entries => this.entries;

        internal protected void AddRange(IEnumerable<T> range)
        {
            this.entries.AddRange(range);
        }

        internal protected void Add(T entry)
        {
            this.entries.Add(entry);
        }
    }
}