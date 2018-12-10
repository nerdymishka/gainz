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
using System.Collections;
using NerdyMishka.Search.IO;

namespace NerdyMishka.Search.Index
{
    /// <summary>
    /// Enumerates over <see cref="DocumentFrequencyPair"/>s
    /// </summary>
    /// <seealso cref="BadMishka.DocumentFormat.LuceneIndex.Index.IDocumentTermEnumerator" />
    public class SegmentTermEnumerator : ITermEnumerator
    {
        /// <summary>
        /// The frequency reader
        /// </summary>
        private IBinaryReader frequencyReader;

        /// <summary>
        /// The frequency count
        /// </summary>
        private int frequencyCount;

        /// <summary>
        /// The deleted docs
        /// </summary>
        private BitVector deletedDocs;

        /// <summary>
        /// The document frequency pair
        /// </summary>
        private DocumentFrequencyPair documentFrequencyPair;

        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentDocumentTermEnumerator"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public SegmentTermEnumerator(SegmentReader owner)
        {
            this.frequencyReader = owner.FrequencyReader;
            this.deletedDocs = owner.DeletedDocs;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentDocumentTermEnumerator"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="termInfo">The term information.</param>
        public SegmentTermEnumerator(SegmentReader owner, TermInfo termInfo)
            : this(owner)
        {
            this.Seek(termInfo);
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value>The current.</value>
        public virtual DocumentFrequencyPair Current => this.documentFrequencyPair;

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value>The current.</value>
        object IEnumerator.Current
        {
            get
            {
                return this.Current;
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            if (this.frequencyReader != null)
            {
                this.frequencyReader.Dispose();
                this.frequencyReader = null;
            }
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
        public virtual bool MoveNext()
        {
            while (true)
            {
                if (this.frequencyCount == 0)
                    return false;

                this.ReadOne();

                if (this.deletedDocs == null || !this.deletedDocs.Get(this.documentFrequencyPair.DocumentId))
                    break;

                this.SkipDocument();
            }

            return true;
        }

        /// <summary>
        /// Reads the number of documents specified by the length of the array
        /// if the enumerator contains the amount given.
        /// </summary>
        /// <param name="documentIndexes">The document indexes.</param>
        /// <param name="frequencies">The frequencies.</param>
        /// <returns>The number of documentIndexes that were read.</returns>
        public virtual int Read(int[] documentIndexes, int[] frequencies)
        {
            int l = documentIndexes.Length,
                i = 0;

            while (i < l && this.frequencyCount > 0)
            {
                this.ReadOne();

                if (this.deletedDocs == null || !this.deletedDocs.Get(this.documentFrequencyPair.DocumentId))
                {
                    documentIndexes[i] = this.documentFrequencyPair.DocumentId;
                    frequencies[i] = this.documentFrequencyPair.TermFrequency;
                    ++i;
                }
            }

            return i;
        }

        /// <summary>
        /// Seeks the specified term information.
        /// </summary>
        /// <param name="termInfo">The term information.</param>
        public virtual void Seek(TermInfo termInfo)
        {
            this.frequencyCount = termInfo.DocumentFrequency;
            this.documentFrequencyPair = new DocumentFrequencyPair(0, 1);
            this.frequencyReader.Seek(termInfo.FrequencyPointer);
        }

        /// <summary>
        /// Skips the enumerator the position where the given documentIndex if found.
        /// Return true on success; otherwise, false.
        /// </summary>
        /// <param name="documentIndex">The document index to move to.</param>
        /// <returns><c>true</c> if found, <c>false</c> otherwise.</returns>
        public virtual bool SkipTo(int documentIndex)
        {
            do
            {
                if (!this.MoveNext())
                    return false;
            }
            while (documentIndex > this.documentFrequencyPair.DocumentId);

            return true;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        /// <exception cref="System.NotImplementedException">This method is not currently implemented</exception>
        public void Reset()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Reads the next <see cref="DocumentFrequencyPair"/>.
        /// </summary>
        protected virtual void ReadOne()
        {
            int docCode = this.frequencyReader.ReadVariableLengthInt32(),
                index = this.documentFrequencyPair.DocumentId,
                frequency = 0;

            index += (int)((uint)docCode) >> 1;

            if ((docCode & 1) != 0)
                frequency = 1;
            else
                frequency = this.frequencyReader.ReadVariableLengthInt32();

            this.documentFrequencyPair = new DocumentFrequencyPair(index, frequency);

            this.frequencyCount--;
        }

        /// <summary>
        /// Skips the document.
        /// </summary>
        protected virtual void SkipDocument()
        {
        }
    }
}
