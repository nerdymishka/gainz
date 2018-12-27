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

namespace NerdyMishka.Search.Index
{
    /// <summary>
    /// Class SegmentListDocumentTermPositionEnumerator.
    /// </summary>
    /// <seealso cref="BadMishka.DocumentFormat.LuceneIndex.Index.SegmentListDocumentTermEnumerator" />
    /// <seealso cref="BadMishka.DocumentFormat.LuceneIndex.Index.IDocumentTermPositionEnumerator" />
    public class MultiSegmentTermPositionEnumerator : MultiSegmentTermEnumerator, ITermPositionEnumerator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentListDocumentTermPositionEnumerator" /> class.
        /// </summary>
        /// <param name="readers">The readers.</param>
        /// <param name="firstDocumentIdForSegments">The first document identifier for segments.</param>
        /// <param name="term">The term.</param>
        public MultiSegmentTermPositionEnumerator(SegmentReader[] readers, int[] firstDocumentIdForSegments, Term term)
            : base(readers, firstDocumentIdForSegments, term)
        {
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
            return ((SegmentTermPositionEnumerator)this.SegmentTermEnumerator).ReadNextPosition();
        }

        /// <summary>
        /// Gets the document term enumerator.
        /// </summary>
        /// <param name="reader">The reader.</param>
        /// <returns>An enumerator for <see cref="DocumentFrequencyPair" /></returns>
        protected override SegmentTermEnumerator GetTermEnumerator(SegmentReader reader)
        {
            return (SegmentTermEnumerator)reader.GetTermPositionsEnumerator(this.Term);
        }
    }
}
