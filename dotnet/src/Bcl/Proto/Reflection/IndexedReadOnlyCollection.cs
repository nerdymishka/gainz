
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NerdyMishka.Reflection
{
    public class IndexedReadOnlyCollection<T> : IReadOnlyCollection<T>
    {
        private Dictionary<string, T> cache = new Dictionary<string, T>();
        private List<T> collection = new List<T>();

        public T this[string key]
        {
            get{
                this.cache.TryGetValue(key, out T value);
                return value;
            }
        }

        internal protected void Add(string key, T value)
        {
            this.cache.Add(key, value);
            if(value != null)
                this.collection.Add(value);
        }

        public int Count => this.collection.Count;

        public IEnumerator<T> GetEnumerator()
        {
            foreach(var item in this.collection)
                yield return item;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}