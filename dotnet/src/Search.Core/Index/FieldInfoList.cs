/*
Copyright 2016 Bad Mishka LLC

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace NerdyMishka.Search.Index
{
    /// <summary>
    /// Class FieldInfoList. The equivalent of <c>FieldInfos</c> in Java.
    /// </summary>
    /// <seealso cref="System.Collections.Generic.IEnumerable{BadMishka.DocumentFormat.LuceneIndex.Index.FieldInfo}" />
    public class FieldInfoList : IEnumerable<FieldInfo>
    {
        private int count;
        private bool hasVectors;
        private static readonly object s_lock = new object();
        /// <summary>
        /// An index by zero based position. In Java, this field is a <c>Vector</c>, which is synchronized and non generic.
        /// </summary>
        private SortedDictionary<int, FieldInfo> numberIndex = new SortedDictionary<int, FieldInfo>();

        /// <summary>
        /// An index by name. In Java, this field is a <c>Hashtable</c>, which is synchronized and non generic.
        /// </summary>
        private Dictionary<string, FieldInfo> namedIndex = new Dictionary<string, FieldInfo>();

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldInfoList"/> class.
        /// </summary>
        public FieldInfoList()
        {   
        }

        /// <summary>
        /// Gets the number of items in the list.
        /// </summary>
        /// <value>The count.</value>
        public int Count => this.namedIndex.Count;

        /// <summary>
        /// Gets a value indicating whether this instance has vectors.
        /// </summary>
        /// <value><c>true</c> if this instance has vectors; otherwise, <c>false</c>.</value>
        public bool HasVectors
        {
            get
            {
                return this.hasVectors;
            }
        }

        /// <summary>
        /// Gets the <see cref="FieldInfo"/> with the specified name.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>A <see cref="FieldInfo"/> object.</returns>
        public FieldInfo this[string name]
        {
            get
            {
                if (this.namedIndex.ContainsKey(name))
                    return this.namedIndex[name];

                return null; 
            }
        }

        /// <summary>
        /// Gets the <see cref="FieldInfo"/> at the specified index.
        /// </summary>
        /// <param name="index">The index of the list.</param>
        /// <returns>A <see cref="FieldInfo"/> object.</returns>
        public FieldInfo this[int index]
        {
            get
            {
                return this.numberIndex[index];
            }
        }

        /// <summary>
        /// Adds a <see cref="FieldInfo"/> by specific values.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isIndexed">if set to <c>true</c> [is indexed].</param>
        /// <param name="isTermVectorStored">if set to <c>true</c> [is term vector stored].</param>
        public void Add(string name, bool isIndexed = true, bool isTermVectorStored = false)
        {
            FieldInfo fi = this[name];
            if (fi == null)
            {
                this.Capture(name, isIndexed, isTermVectorStored);
                return;
            }

            // once indexed, always index ಠ_ಠ
            if (fi.IsIndexed != isIndexed)
                fi.IsIndexed = true;

            // once vector, always vector ಠ_ಠ
            if (fi.IsTermVectorStored != isTermVectorStored)
                fi.IsTermVectorStored = true; 

            if(fi.IsTermVectorStored)
                hasVectors = true;    
        }

        /// <summary>
        /// Adds a range of <see cref="FieldInfo"/>s.
        /// </summary>
        /// <param name="fields">The fields.</param>
        public void AddRange(IEnumerable<FieldInfo> fields)
        {
            foreach (var fieldInfo in fields)
            {
                this.Add(fieldInfo.Name, fieldInfo.IsIndexed);
            }
        }

        /// <summary>
        /// Adds a range of <see cref="FieldInfo"/> by specifying the values.
        /// </summary>
        /// <param name="names">The names.</param>
        /// <param name="isIndexed">if set to <c>true</c> [is indexed].</param>
        /// <param name="isTermVectorStored">if set to <c>true</c> [is term vector stored].</param>
        public void AddRange(IEnumerable<string> names, bool isIndexed = true, bool isTermVectorStored = false)
        {
            foreach (var name in names)
            {
                this.Add(name, isIndexed, isTermVectorStored);
            }
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>A <see cref="T:System.Collections.Generic.IEnumerator`1" /> that can be used to iterate through the collection.</returns>
        public IEnumerator<FieldInfo> GetEnumerator()
        {
            return this.numberIndex.Values.GetEnumerator();
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>An <see cref="T:System.Collections.IEnumerator" /> object that can be used to iterate through the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.numberIndex.Values.GetEnumerator();
        }

        /// <summary>
        /// Returns the index of a given field by name.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>The index of the given field if found; otherwise, -1.</returns>
        public int IndexOf(string fieldName)
        {
            var fieldInfo = this[fieldName];

            if (fieldInfo == null)
                return -1;

            return fieldInfo.Index;
        }

        /// <summary>
        /// Captures the specified <see cref="FieldInfo"/> and stores it.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <param name="isIndexed">if set to <c>true</c> [is indexed].</param>
        /// <param name="storeTermVector">if set to <c>true</c> [store term vector].</param>
        internal void Capture(string name, bool isIndexed = true, bool storeTermVector = true)
        {
            lock(s_lock)
            {
                var fi = new FieldInfo(name, isIndexed, this.count, storeTermVector);
                this.numberIndex.Add(fi.Index, fi);
                this.namedIndex.Add(fi.Name, fi);
            } 
        }
    }
}
