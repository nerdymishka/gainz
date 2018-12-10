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

namespace BadMishka.DocumentFormat.LuceneIndex.Index
{
    /// <summary>
    /// Class SegmentListDocumentTermEnumerator.
    /// </summary>
    /// <seealso cref="BadMishka.DocumentFormat.LuceneIndex.Index.IDocumentTermEnumerator" />
    public class SegmentListDocumentTermEnumerator : ITermEnumerator
    {
        /// <summary>
        /// The readers
        /// </summary>
        private SegmentReader[] readers;

        /// <summary>
        /// The first document identifier for segments
        /// </summary>
        private int[] firstDocumentIdForSegments;

        /// <summary>
        /// The term
        /// </summary>
        private Term term;

        /// <summary>
        /// The base value
        /// </summary>
        private int baseValue = 0;

        /// <summary>
        /// The position
        /// </summary>
        private int position = 0;

        /// <summary>
        /// The segment document term enumerator
        /// </summary>
        private SegmentTermEnumerator segmentTermEnumerator;

        /// <summary>
        /// The current
        /// </summary>
        private DocumentFrequencyPair current;

        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentListDocumentTermEnumerator"/> class.
        /// </summary>
        /// <param name="readers">The readers.</param>
        /// <param name="firstDocumentIdForSegments">The first document identifier for segments.</param>
        /// <param name="term">The term.</param>
        public SegmentListDocumentTermEnumerator(SegmentReader[] readers, int[] firstDocumentIdForSegments, Term term)
        {
            this.readers = readers;
            this.firstDocumentIdForSegments = firstDocumentIdForSegments;
            this.term = term;
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value>The current.</value>
        public DocumentFrequencyPair Current => this.current;

        protected Term Term => this.term;
        
        /// <summary>
        /// Gets the segment document term enumerator.
        /// </summary>
        /// <value>The segment document term enumerator.</value>
        protected SegmentDocumentTermEnumerator SegmentDocumentTermEnumerator => segmentDocumentTermEnumerator;

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
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
        public bool MoveNext()
        {
            var enumerator = this.segmentDocumentTermEnumerator;
            if (enumerator != null && enumerator.MoveNext())
            {
                this.current = enumerator.Current;
                return true;
            }
            else if (this.position < this.readers.Length)
            {
                if (enumerator != null)
                    enumerator.Dispose();

                this.baseValue = this.firstDocumentIdForSegments[this.position];
                this.segmentDocumentTermEnumerator = this.GetDocumentTermEnumerator(this.readers[this.position++]);
                return this.MoveNext();
            }

            return false;
        }

        /// <summary>
        /// Reads the specified document ids.
        /// </summary>
        /// <param name="documentIds">The document ids.</param>
        /// <param name="frequencies">The frequencies.</param>
        /// <returns>The amount read.</returns>
        public int Read(int[] documentIds, int[] frequencies)
        {
            while (true)
            {
                while (this.segmentDocumentTermEnumerator == null)
                {
                    if (this.position < this.readers.Length)
                    {
                        this.baseValue = this.firstDocumentIdForSegments[this.position];
                        this.segmentDocumentTermEnumerator = this.GetDocumentTermEnumerator(this.readers[this.position++]);
                    }
                    else
                    {
                        return 0;
                    }
                }

                int end = this.segmentDocumentTermEnumerator.Read(documentIds, frequencies);
                if (end == 0)
                {
                    this.segmentDocumentTermEnumerator.Dispose();
                    this.segmentDocumentTermEnumerator = null;
                }
                else
                {
                    for (int i = 0; i < end; i++)
                        documentIds[i] += this.baseValue;

                    return end;
                }
            }
        }

        /// <summary>
        /// Skips entries to the first beyond the current whose document number is
        /// greater than or equal to *target*. Returns true if the <param name="documentIndex" />
        /// was found.
        /// </summary>
        /// <param name="documentIndex">Index of the document.</param>
        /// <returns><c>true</c> if XXXX, <c>false</c> otherwise.</returns>
        /// <remarks><para>
        /// Implementations MAY vary.
        /// </para>
        /// <code lang="csharp">
        /// private boolean SkipTo(int documentIndex) {
        /// do {
        /// if (!this.MoveNext())
        /// return false;
        /// } while (target &gt; this.Current.Index);
        /// return true;
        /// }
        /// </code></remarks>
        public bool SkipTo(int documentIndex)
        {
            do
            {
                if (!this.MoveNext())
                    return false;
            }
            while (documentIndex > this.current.DocumentId);

            return true;
        }

        /// <summary>
        /// Resets this instance.
        /// </summary>
        public void Reset()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            if (this.segmentDocumentTermEnumerator != null)
                this.segmentDocumentTermEnumerator.Dispose();
        }

        /// <summary>
        /// Gets the document term enumerator.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>An enumerator for <see cref="DocumentFrequencyPair"/></returns>
        protected virtual SegmentDocumentTermEnumerator GetDocumentTermEnumerator(SegmentReader reader)
        {
            return (SegmentDocumentTermEnumerator)reader.GetDocumentTermsEnumerator(this.term);
        }
    }
}
