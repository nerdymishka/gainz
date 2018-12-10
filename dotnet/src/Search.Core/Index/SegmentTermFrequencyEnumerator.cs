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
using System;
using System.Diagnostics;
using NerdyMishka.Search.IO;

namespace NerdyMishka.Search.Index
{
    /// <summary>
    /// Class SegmentTermsEnumerator.
    /// </summary>
    /// <seealso cref="BadMishka.DocumentFormat.LuceneIndex.Index.TermEnumeratorBase" />
    /// <seealso cref="BadMishka.DocumentFormat.LuceneIndex.ICloneable" />
    public class SegmentTermFrequencyEnumerator : TermFrequencyEnumeratorBase, ICloneable
    {
        /// <summary>
        /// The <see cref="char[]"/> buffer
        /// </summary>
        private char[] buffer = new char[] { };

        /// <summary>
        /// The number of items in the enumerator
        /// </summary>
        private long count;

        /// <summary>
        /// The field information list
        /// </summary>
        private FieldInfoList fieldInfoList;

        /// <summary>
        /// The pointer to the index.
        /// </summary>
        private long indexPointer = 0;

        /// <summary>
        /// Is the segment an index?
        /// </summary>
        private bool isIndex = false;

        /// <summary>
        /// The current position of the enumerator, which is a zero-based index.
        /// </summary>
        private long position = -1;

        /// <summary>
        /// The previous term
        /// </summary>
        private Term previousTerm;

        /// <summary>
        /// The binary reader
        /// </summary>
        private IBinaryReader reader;

        /// <summary>
        /// The term/]
        /// </summary>
        private TermFrequencyPair termFrequency;

        /// <summary>
        /// The term information
        /// </summary>
        private TermInfo termInfo = new TermInfo();

        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentTermFrequencyEnumerator"/> class.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <param name="fieldInfoList">The field information list.</param>
        /// <param name="isIndex">if set to <c>true</c> [is index].</param>
        public SegmentTermFrequencyEnumerator(IBinaryReader reader, FieldInfoList fieldInfoList, bool isIndex)
        {
            this.termFrequency = new TermFrequencyPair(new Term(string.Empty, string.Empty), this.termInfo.DocumentFrequency);
            this.reader = reader;
            this.fieldInfoList = fieldInfoList;
            this.isIndex = isIndex;
            this.count = this.reader.ReadInt32();
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value>The current.</value>
        public override TermFrequencyPair Current => this.termFrequency;

        /// <summary>
        /// Gets the term information.
        /// </summary>
        /// <value>The term information.</value>
        public TermInfo TermInfo => new TermInfo(this.termInfo);

        /// <summary>
        /// Gets the number of items in the enumerator
        /// </summary>
        /// <value>The count.</value>
        internal long Count => this.count;

        /// <summary>
        /// Gets the frequency pointer.
        /// </summary>
        /// <value>The frequency pointer.</value>
        internal long FrequencyPointer => this.termInfo.FrequencyPointer;

        /// <summary>
        /// Gets the index pointer.
        /// </summary>
        /// <value>The index pointer.</value>
        internal long IndexPointer => this.indexPointer;

        /// <summary>
        /// Gets the position.
        /// </summary>
        /// <value>The position.</value>
        internal long Position => this.position;

        /// <summary>
        /// Gets the previous term.
        /// </summary>
        /// <value>The previous term.</value>
        internal Term PreviousTerm => this.previousTerm;

        /// <summary>
        /// Gets the proxy pointer.
        /// </summary>
        /// <value>The proxy pointer.</value>
        internal long ProxyPointer => this.termInfo.ProxyPointer;

        /// <summary>
        /// Clones the instance of the current object.
        /// </summary>
        /// <param name="deep">If true, a deep clone is requested.</param>
        /// <returns>A clone of the instance of the current object.</returns>
        public object Clone()
        {
            SegmentTermFrequencyEnumerator enumerator = null;

            try
            {
                enumerator = (SegmentTermFrequencyEnumerator)this.MemberwiseClone();
            }
            catch (Exception ex)
            {
                Debug.Write(ex);
            }

            enumerator.reader = (IBinaryReader)this.reader.Clone();
            enumerator.termInfo = this.TermInfo;

            if (this.termFrequency.Term != null)
                enumerator.GrowBuffer(this.termFrequency.Term.Text.Length);

            return enumerator;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            this.reader.Dispose();
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
        public override bool MoveNext()
        {
            if (this.position++ >= this.count - 1)
            {
                this.termFrequency = null;
                return false;
            }

            this.previousTerm = this.termFrequency.Term;
            this.termFrequency = this.Read();

            if (this.isIndex)
                this.indexPointer += this.reader.ReadVariableLengthInt64();

            return true;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        public override void Reset()
        {
            this.Seek(0, 0, new Term(string.Empty, string.Empty), new TermInfo());
        }

        /// <summary>
        /// Seeks the specified pointer.
        /// </summary>
        /// <param name="pointer">The pointer.</param>
        /// <param name="position">The position.</param>
        /// <param name="term">The term.</param>
        /// <param name="termInfo">The term information.</param>
        internal void Seek(long pointer, int position, Term term, TermInfo termInfo)
        {
            this.reader.Seek(pointer);
            this.position = position;
            this.termFrequency = new TermFrequencyPair(term, termInfo.DocumentFrequency);
            this.previousTerm = null;
            this.termInfo.Set(termInfo);
            this.GrowBuffer(term.Text.Length);
        }

        /// <summary>
        /// Grows the buffer.
        /// </summary>
        /// <param name="length">The length.</param>
        private void GrowBuffer(int length)
        {
            this.buffer = new char[length];
            var text = this.termFrequency.Term.Text;
            int i = 0,
                l = text.Length;

            for (; i < l; i++)
                this.buffer[i] = text[i];
        }

        /// <summary>
        /// Reads the term from the binary reader.
        /// </summary>
        /// <returns>A <see cref="Term"/></returns>
        /// <exception cref="System.IndexOutOfRangeException">Thrown when a field cannot be found by the index read from the reader</exception>
        private TermFrequencyPair Read()
        {
            int start = this.reader.ReadVariableLengthInt32(),
                length = this.reader.ReadVariableLengthInt32(),
                totalLength = start + length;

            if (this.buffer.Length < totalLength)
                this.GrowBuffer(totalLength);

            this.reader.Read(this.buffer, start, length);

            var fieldIndex = this.reader.ReadVariableLengthInt32();
            var field = this.fieldInfoList[fieldIndex];

            if (field == null)
                throw new IndexOutOfRangeException($"A fieldInfo could not be found at index {fieldIndex}");

            var term = new Term(field.Name, new string(this.buffer, 0, totalLength), false);

            var documentFrequency = this.reader.ReadVariableLengthInt32();
            var frequencyPointer = this.reader.ReadVariableLengthInt64();
            var proxyPointer = this.reader.ReadVariableLengthInt64();

            this.termInfo.Set(
                documentFrequency,
                frequencyPointer,
                proxyPointer);

            return new TermFrequencyPair(term, this.termInfo.DocumentFrequency);
        }
    }
}
