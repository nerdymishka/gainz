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

namespace BadMishka.DocumentFormat.LuceneIndex.Index
{
    /// <summary>
    /// Class SegmentListTermFrequencyEnumerator./
    /// </summary>
    /// <seealso cref="BadMishka.DocumentFormat.LuceneIndex.Index.TermFrequenciesEnumeratorBase" />
    internal class SegmentListTermFrequencyEnumerator : TermFrequenciesEnumeratorBase
    {
        /// <summary>
        /// The queue
        /// </summary>
        private SegmentMergeQueue queue;

        /// <summary>
        /// The current
        /// </summary>
        private TermFrequencyPair current;

        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentListTermFrequencyEnumerator"/> class.
        /// </summary>
        /// <param name="readers">The readers.</param>
        /// <param name="firstDocumentIdForSegments">The first document identifier for segments.</param>
        /// <param name="term">The term.</param>
        public SegmentListTermFrequencyEnumerator(SegmentReader[] readers, int[] firstDocumentIdForSegments, Term term)
        {
            int i = 0,
               l = readers.Length;
            this.queue = new SegmentMergeQueue(l);
           
            for (; i < l; i++)
            {
                var reader = readers[i];
                SegmentTermFrequencyEnumerator enumerator = null;

                if (term != null)
                    enumerator = (SegmentTermFrequencyEnumerator)reader.GetTermsEnumerator(term);
                else
                    enumerator = (SegmentTermFrequencyEnumerator)reader.GetTermsEnumerator();

                var segmentMergeInfo = new SegmentMergeInfo(firstDocumentIdForSegments[i], enumerator, reader);

                if (term == null ? segmentMergeInfo.MoveNext() : enumerator.Current != null)
                    this.queue.Put(segmentMergeInfo);
                else
                    segmentMergeInfo.Dispose();
            }

            if (term != null && this.queue.Count > 0)
            {
                var top = this.queue.Top();
                var currentTerm = top.TermsEnumerator?.Current?.Term;
                var frequency = top.TermsEnumerator.Current.Frequency;
                this.current = new TermFrequencyPair(currentTerm, frequency);
            }
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <value>The current.</value>
        public override TermFrequencyPair Current => this.current;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public override void Dispose()
        {
            if (this.queue != null)
                this.queue.Dispose();
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.</returns>
        public override bool MoveNext()
        {
            var top = this.queue.Top();
            if (top == null)
            {
                this.current = null;
                return false;
            }

            Term term = top.Term;
            int frequency = 0;

            while (top != null && term.CompareTo(top.Term) == 0)
            {
                this.queue.Pop();
                frequency += top.TermsEnumerator.Current.Frequency;

                if (top.MoveNext())
                    this.queue.Put(top);
                else
                    top.Dispose();

                top = this.queue.Top();
            }

            this.current = new TermFrequencyPair(term, frequency);

            return true;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        /// <exception cref="System.NotImplementedException">Not Implemented</exception>
        public override void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
