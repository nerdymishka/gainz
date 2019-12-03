using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.Search.Collections
{
    public class ThreadSafeList<T> : IList<T>, IProducerConsumerCollection<T>
    {
        protected List<T> list = new List<T>();
        protected object syncRoot = new object();

        public ThreadSafeList()
        {

        }

        public ThreadSafeList(int capacity)
        {
            this.list = new List<T>(capacity);
        }

        public ThreadSafeList(IEnumerable<T> items)
        {
            foreach (var item in items)
                this.Add(item);
        }

        

        public T this[int index]
        {
            get
            {
                lock (syncRoot)
                {
                    if (index < 0 || index >= this.list.Count)
                        throw new ArgumentOutOfRangeException(nameof(index));

                    return this.list[index];
                }
            }

            set
            {
                lock (syncRoot)
                {
                    if (index >= this.list.Count)
                        throw new ArgumentOutOfRangeException(nameof(index));

                    this.list[index] = value;
                }
            }
        }

        public int Count
        {
            get {
                lock (syncRoot) { return this.list.Count; }
            }
            set
            {
                // Allows Vector.java like functionality that pops off values from the end of the list.
                lock (syncRoot)
                {
                    if (value == this.list.Count)
                        return;

                    if (value > this.list.Count)
                    {
                        int i = this.list.Count - 1,
                            l = value - this.list.Count;

                        for (; i < l; i++)
                        {
                            this.list.Add(default(T));
                        }
                    }
                    else
                    {
                        int l = this.list.Count - value,
                            i = this.list.Count - 1;

                        for (; l > i; i--)
                        {
                            this.list.RemoveAt(i);
                        }
                    }
                }
            }

        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return true;
            }
        }

        public object SyncRoot => this.syncRoot;

        public void Add(T item)
        {
            lock(syncRoot)
            {
                this.list.Add(item);
            }
        }

        public void AddRange(IEnumerable<T> items)
        {
            lock(syncRoot)
            {
                this.list.AddRange(items);
            }
        }

        public void Clear()
        {
            lock(syncRoot)
            {
                this.list.Clear();
            }
        }

        public bool Contains(T item)
        {
            lock(syncRoot)
            {
                return this.list.Contains(item);
            }
           
        }

        public void CopyTo(Array array, int index)
        {
            lock(syncRoot)
            {
                var copy = this.list.ToArray();
                Array.Copy(copy, 0, array, index, array.Length);
            }
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
           lock(syncRoot)
            {
                this.list.CopyTo(array, arrayIndex);
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            lock(syncRoot)
            {
                var copy = new List<T>(this.list);
                return copy.GetEnumerator();
            }
        }

        public int IndexOf(T item)
        {
            lock(syncRoot)
            {
                return this.list.IndexOf(item);
            }
        }

        public void Insert(int index, T item)
        {
            lock(syncRoot)
            {
                this.list.Insert(index, item);
            }
        }

        public bool Remove(T item)
        {
            lock(syncRoot)
            {
                return this.list.Remove(item);
            }
        }

        public void RemoveAt(int index)
        {
            lock(syncRoot)
            {
                this.list.RemoveAt(index);
            }
        }

        public void RemoveRange(int index, int count)
        {
            lock(syncRoot)
            {
                this.list.RemoveRange(index, count);
            }
        }

        public T[] ToArray()
        {
            lock(syncRoot)
            {
                var copy = this.list.ToArray();
                return copy;
            }
        }

        public bool TryAdd(T item)
        {
            lock(syncRoot)
            {
                this.list.Add(item);
                return true;
            }
        }

        public bool TryTake(out T item)
        {
            lock(syncRoot)
            {
                item = default(T);
                if (this.list.Count == 0)
                    return false;

                int lastIndex = this.list.Count - 1;
                item = this.list[lastIndex];

                this.list.Remove(item);

                return true;
            }
        }


        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}
