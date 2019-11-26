using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;

namespace NerdyMishka.Reflection
{

   

    public class GroupedReadOnlyCollection<T> : IReadOnlyCollection<T> where T: class, new()
    {
        private List<T> attributes = new List<T>();
        private List<GroupEntry<T>> indexes = new List<GroupEntry<T>>();
        private List<T> inherited = new List<T>();


        internal protected void AddRange<S>(IEnumerable<S> range) where S:T 
        {
            var t = typeof(T);
            var entry = this.indexes.SingleOrDefault(o => o.Type == t);
            if(entry == null) {
                entry = new GroupEntry<T>() { 
                    Type = t,
                    Name = t.Name,
                    FullName = t.FullName
                };
                this.indexes.Add(entry);
            }
            entry.AddRange(range.Cast<T>());
            this.attributes.AddRange(range.Cast<T>());
        }

        internal protected void Add<S>(S attribute) where S:T 
        {
            var t = attribute.GetType();
            var entry = this.indexes.SingleOrDefault(o => o.Type == t);
            if(entry == null) {
                entry = new GroupEntry<T>() { 
                    Type = t,
                    Name = t.Name,
                    FullName = t.FullName
                };
                this.indexes.Add(entry);
            }
            entry.Add(attribute);
            this.attributes.Add(attribute);
        }

        public S FindFirst<S>() where S:T 
        {
            return (S)this.attributes.FirstOrDefault(o => o is S);
        } 

        public T FindFirst(Type type)
        {
            return (T)this.attributes.FirstOrDefault(o => o.GetType() == type);
        }

        public IEnumerable<S> FindGroup<S>() where S: T
        {
            var t = typeof(S);
            var group = this.indexes.SingleOrDefault(o => o.Type == t);
            if(group != null)
                return group.Entries.Cast<S>();

            return Enumerable.Empty<S>();
        }

        public IEnumerable<T> FindGroup(Type type)
        {
            var group = this.indexes.SingleOrDefault(o => o.Type == type);
            if(group != null)
                return group.Entries;

            return Enumerable.Empty<T>();
        }

        public int Count => this.attributes.Count;

        public IEnumerator<T> GetEnumerator()
        {
            foreach(var item in this.attributes)
                yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}