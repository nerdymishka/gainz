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
using BadMishka.DocumentFormat.LuceneIndex.Store;

namespace BadMishka.DocumentFormat.LuceneIndex.Index
{
    /// <summary>
    /// Class SegmentDocumentTermPositionsEnumerator.  This is <c>SegmentTermPositions</c> in Java.
    /// </summary>
    /// <seealso cref="BadMishka.DocumentFormat.LuceneIndex.Index.SegmentDocumentTermEnumerator" />
    /// <seealso cref="BadMishka.DocumentFormat.LuceneIndex.Index.IDocumentTermPositionEnumerator" />
    public class SegmentDocumentTermPositionEnumerator : SegmentDocumentTermEnumerator, IDocumentTermPositionEnumerator
    {
        /// <summary>
        /// The proxy reader
        /// </summary>
        private ILuceneBinaryReader proxyReader;

        /// <summary>
        /// The proxy count
        /// </summary>
        private int proxyCount;

        /// <summary>
        /// The position
        /// </summary>
        private int position;

        /// <summary>
        /// The has moved
        /// </summary>
        private bool hasMoved = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentDocumentTermPositionEnumerator"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        public SegmentDocumentTermPositionEnumerator(SegmentReader owner) 
            : base(owner)
        {
            this.proxyReader = owner.ProxyReader;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentDocumentTermPositionEnumerator"/> class.
        /// </summary>
        /// <param name="owner">The owner.</param>
        /// <param name="termInfo">The term information.</param>
        public SegmentDocumentTermPositionEnumerator(SegmentReader owner, TermInfo termInfo)
            : base(owner)
        {
            this.Seek(termInfo);
        }

        /// <summary>
        /// Seeks the specified term information.
        /// </summary>
        /// <param name="termInfo">The term information.</param>
        public override void Seek(TermInfo termInfo)
        {
            base.Seek(termInfo);
            this.proxyReader.Seek(termInfo.FrequencyPointer);
        }

        /// <summary>
        /// Reads the number of documents specified by the length of the array
        /// if the enumerator contains the amount given. (Not Implemented).
        /// </summary>
        /// <param name="documentIndexes">The document indexes.</param>
        /// <param name="frequencies">The frequencies.</param>
        /// <returns>The number of documentIndexes that were read.</returns>
        /// <exception cref="System.NotSupportedException">This method is not Implemented.</exception>
        public override int Read(int[] documentIndexes, int[] frequencies)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
        public override bool MoveNext()
        {
            for (int f = this.proxyCount; f > 0; f--)
                this.proxyReader.ReadVariableLengthInt32();

            if (base.MoveNext())
            {
                this.hasMoved = true;
                this.proxyCount = this.Current.Frequency;
                this.position = 0;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Reads next position in the current document.  It is an error to call
        /// this more than <see cref="ITermDocs.Frequency" /> times
        /// without calling <see cref="ITermDocs.Next" /> This is
        /// invalid until <see cref="ITermDocs.Next" /> is called for
        /// the first time.
        /// </summary>
        /// <returns>The next position; otherwise -1</returns>
        public int ReadNextPosition()
        {
            if (!this.hasMoved)
                throw new InvalidOperationException("MoveNext() must be called first");

            this.hasMoved = false;
            this.proxyCount--;
            return this.position += this.proxyReader.ReadInt32();
        }

        /// <summary>
        /// Skips the document.
        /// </summary>
        protected override void SkipDocument()
        {
            for (int f = this.Current.Frequency; f > 0; f--)
                this.proxyReader.ReadVariableLengthInt32();
        }
    }
}
