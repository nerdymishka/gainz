namespace NerdyMishka.Search.Collections
{
    public abstract class PriorityQueue<T> where T: System.IComparable<T>
    {
        private T[] heap;
        private int size;
        private int maxSize;

        /// <summary>Determines the ordering of objects in this priority queue.  Subclasses
        /// must define this one method. 
        /// </summary>
        public virtual bool LessThan(T left, T right)
        {
            return left.CompareTo(right) == -1;
        }

        /// <summary>Subclass constructors must call this. </summary>
        protected internal void Initialize(int maxLength)
        {
            size = 0;
            int heapSize = maxLength + 1;
            heap = new T[heapSize];
            this.maxSize = maxLength;
        }

        /// <summary> Adds an Object to a PriorityQueue in log(size) time.
        /// If one tries to add more objects than maxSize from initialize
        /// a RuntimeException (ArrayIndexOutOfBound) is thrown.
        /// </summary>
        public void Put(T element)
        {
            size++;
            heap[size] = element;
            UpHeap();
        }

        /// <summary> Adds element to the PriorityQueue in log(size) time if either
        /// the PriorityQueue is not full, or not lessThan(element, top()).
        /// </summary>
        /// <param name="">element
        /// </param>
        /// <returns> true if element is added, false otherwise.
        /// </returns>
        public virtual bool Insert(T element)
        {
            if (size < maxSize)
            {
                Put(element);
                return true;
            }
            else if (size > 0 && !LessThan(element, Top()))
            {
                heap[1] = element;
                AdjustTop();
                return true;
            }
            else
                return false;
        }

        /// <summary>Returns the least element of the PriorityQueue in constant time. </summary>
        public T Top()
        {
            if (size > 0)
                return heap[1];
            else
                return default(T);
        }

        /// <summary>Removes and returns the least element of the PriorityQueue in log(size)
        /// time. 
        /// </summary>
        public T Pop()
        {
            if (size > 0)
            {
                T result = heap[1]; // save first value
                heap[1] = heap[size]; // move last to first
                heap[size] = default(T); // permit GC of objects
                size--;
                DownHeap(); // adjust heap
                return result;
            }
            else
                return default(T);
        }

        /// <summary>Should be called when the Object at top changes values.  Still log(n)
        /// worst case, but it's at least twice as fast to <pre>
        /// { pq.top().change(); pq.adjustTop(); }
        /// </pre> instead of <pre>
        /// { o = pq.pop(); o.change(); pq.push(o); }
        /// </pre>
        /// </summary>
        public void AdjustTop()
        {
            DownHeap();
        }


        /// <summary>Returns the number of elements currently stored in the PriorityQueue. </summary>
        public int Count => this.size;

        /// <summary>Removes all entries from the PriorityQueue. </summary>
        public void Clear()
        {
            for (int i = 0; i <= size; i++)
                heap[i] = default(T);
            size = 0;
        }

        private void UpHeap()
        {
            int i = size;
            T node = heap[i]; // save bottom node
            int j = (int)(((uint)i) >> 1);
            while (j > 0 && LessThan(node, heap[j]))
            {
                heap[i] = heap[j]; // shift parents down
                i = j;
                j = (int)(((uint)j) >> 1);
            }
            heap[i] = node; // install saved node
        }

        private void DownHeap()
        {
            int i = 1;
            T node = heap[i]; // save top node
            int j = i << 1; // find smaller child
            int k = j + 1;
            if (k <= size && LessThan(heap[k], heap[j]))
            {
                j = k;
            }
            while (j <= size && LessThan(heap[j], node))
            {
                heap[i] = heap[j]; // shift up child
                i = j;
                j = i << 1;
                k = j + 1;
                if (k <= size && LessThan(heap[k], heap[j]))
                {
                    j = k;
                }
            }
            heap[i] = node; // install saved node
        }
    }
}