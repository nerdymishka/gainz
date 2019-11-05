/**
 * Copyright 2016 Bad Mishka LLC
 * Based Upon Lucene from The Apache Foundation, Copyright 2004
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
using System;

namespace NerdyMishka.Search.Index
{
    /// <summary>
    /// Class SegmentMergeInfo. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="System.IComparable{BadMishka.DocumentFormat.LuceneIndex.Index.SegmentMergeInfo}" />
    /// <seealso cref="System.IDisposable" />
    sealed class SegmentMergeInfo : IComparable<SegmentMergeInfo>, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentMergeInfo"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <param name="termsEnumerator">The terms enumerator.</param>
        /// <param name="reader">The reader.</param>
        internal SegmentMergeInfo(int index, ITermFrequencyEnumerator termsEnumerator, SegmentReader reader)
        {
            this.Index = index;
            this.TermFrequencyEnumerator = termsEnumerator;
            this.Term = termsEnumerator.Current.Term;
            this.TermPositionEnumerator = reader.GetTermPositionsEnumerator(null);

            // build array which maps document numbers around deletions 
            if (reader.HasDeletions)
            {
                int maxDoc = reader.TotalDocumentCount;
                this.DocumentIndexMap = new int[maxDoc];
                int j = 0;
                for (int i = 0; i < maxDoc; i++)
                {
                    if (reader.IsDocumentDeleted(i))
                        this.DocumentIndexMap[i] = -1;
                    else
                        this.DocumentIndexMap[i] = j++;
                }
            }
        }

        /// <summary>
        /// Gets or sets the document index map.
        /// </summary>
        /// <value>The document index map.</value>
        internal int[] DocumentIndexMap { get; private set; }

        /// <summary>
        /// Gets the document terms position enumerator.
        /// </summary>
        /// <value>The document terms position enumerator.</value>
        internal ITermPositionEnumerator TermPositionEnumerator { get; private set; }

        /// <summary>
        /// Gets or sets the index.
        /// </summary>
        /// <value>The index.</value>
        internal int Index { get; private set; }

        /// <summary>
        /// Gets or sets the term.
        /// </summary>
        /// <value>The term.</value>
        internal Term Term { get; private set; }

        /// <summary>
        /// Gets the terms enumerator.
        /// </summary>
        /// <value>The terms enumerator.</value>
        internal ITermFrequencyEnumerator TermFrequencyEnumerator { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.TermFrequencyEnumerator != null)
                this.TermFrequencyEnumerator.Dispose();

            if (this.TermPositionEnumerator != null)
                this.TermPositionEnumerator.Dispose();
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.</returns>
        public int CompareTo(SegmentMergeInfo other)
        {
            int comparison = this.Term.CompareTo(other.Term);
            if (comparison == 0)
                return this.Index.CompareTo(other.Index);
            else
                return comparison;
        }

        /// <summary>
        /// Moves to the next term in the enumerator.
        /// </summary>
        /// <returns><c>true</c> if a next item was found; otherwise, <c>false</c></returns>
        internal bool MoveNext()
        {
            if (this.TermFrequencyEnumerator.MoveNext())
            {
                this.Term = this.TermFrequencyEnumerator.Current.Term;
                return true;
            }
            else
            {
                this.Term = null;
                return false;
            }
        }
    }
}
