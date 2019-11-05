using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NerdyMishka.Search.Documents;

namespace NerdyMishka.Search.Index
{
    /// <summary>
    /// Class SegmentListReader.
    /// </summary>
    /// <seealso cref=".IndexReader" />
    public class MultiSegmentReader : IndexReader
    {
        /// <summary>
        /// The synchronize root
        /// </summary>
        private readonly object syncRoot = new object();

        /// <summary>
        /// The readers
        /// </summary>
        private SegmentReader[] readers;

        /// <summary>
        /// The first document identifier for segments
        /// </summary>
        private int[] firstDocumentIdForSegments;

        /// <summary>
        /// The normalized factors
        /// </summary>
        private Dictionary<string, byte[]> normalizedFactors = new Dictionary<string, byte[]>();

        /// <summary>
        /// The document count
        /// </summary>
        private int documentCount = 0;

        /// <summary>
        /// The total document count
        /// </summary>
        private int totalDocumentCount = 0;

        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentListReader"/> class.
        /// </summary>
        /// <param name="readers">The readers.</param>
        public MultiSegmentReader(SegmentReader[] readers)
        {
            this.readers = readers;
            int i = 0,
                l = this.readers.Length;
            this.firstDocumentIdForSegments = new int[l + 1];

            for (; i < l; i++)
            {
                this.firstDocumentIdForSegments[i] = this.totalDocumentCount;
                this.totalDocumentCount += this.readers[i].TotalDocumentCount;
            }

            this.firstDocumentIdForSegments[l] = this.totalDocumentCount;
        }

        /// <summary>
        /// The number of active documents
        /// </summary>
        /// <value>The count.</value>
        public override int Count
        {
            get
            {
                if (this.documentCount == -1)
                {
                    int count = 0,
                        i = 0,
                        l = this.readers.Length;

                    for (; i < l; i++)
                    {
                        count += this.readers[i].Count;
                    }
                }

                return this.documentCount;
            }
        }

        /// <summary>
        /// Gets a value indicating whether this instance has deletions.
        /// </summary>
        /// <value><c>true</c> if this instance has deletions; otherwise, <c>false</c>.</value>
        public bool HasDeletions
        {
            get
            {
                return this.TotalDocumentCount != this.Count;
            }
        }

        /// <summary>
        /// The total number of documents, including documents waiting to be deleted.
        /// </summary>
        /// <value>The total document count.</value>
        public override int TotalDocumentCount => this.totalDocumentCount;

        /// <summary>
        /// Gets the <see cref="Document"/> with the specified document identifier.
        /// </summary>
        /// <param name="documentId">The document identifier.</param>
        /// <returns>a <see cref="Document"/>.</returns>
        public override Document this[int documentId]
        {
            get
            {
                var index = this.IndexOf(documentId);
                return this.readers[index][documentId = this.firstDocumentIdForSegments[index]];
            }
        }

        /// <summary>
        /// Deletes the specified document by id.
        /// </summary>
        /// <param name="documentId">The document identifier.</param>
        public override void Delete(int documentId)
        {
            this.documentCount = -1;
            int i = this.IndexOf(documentId);
            this.readers[i].Delete(documentId = this.firstDocumentIdForSegments[i]);
        }

        /// <summary>
        /// Gets the document frequency.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns>The document frequency.</returns>
        public override int GetDocumentFrequency(Term term)
        {
            int total = 0,
                i = 0,
                l = this.readers.Length;

            for (; i < l; i++)
            {
                total += this.readers[i].GetDocumentFrequency(term);
            }

            return total;
        }

        /// <summary>
        /// Determines whether [is document deleted] [the specified index].
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns><c>true</c> if [is document deleted] [the specified index]; otherwise, <c>false</c>.</returns>
        public override bool IsDocumentDeleted(int index)
        {
            int i = this.IndexOf(index);
            return this.readers[i].IsDocumentDeleted(index - this.firstDocumentIdForSegments[i]);
        }

        /// <summary>
        /// Normalizations the factor.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>The normalized bytes.</returns>
        public override byte[] ReadNormalizedBytes(string fieldName)
        {
            var bytes = this.normalizedFactors[fieldName];
            if (bytes != null)
                return bytes;

            bytes = new byte[this.TotalDocumentCount];
            for (int i = 0; i < this.readers.Length; i++)
                this.readers[i].ReadNormalizedBytes(fieldName, bytes, this.firstDocumentIdForSegments[i]);

            this.normalizedFactors[fieldName] = bytes;

            return bytes;
        }

        /// <summary>
        /// Gets the terms enumerator.
        /// </summary>
        /// <param name="termToSeek">The term to seek.</param>
        /// <returns>An enumerator of terms and frequencies</returns>
        public override ITermFrequencyEnumerator GetTermFrequencyEnumerator(Term termToSeek = null)
        {
            return new MultiSegmentTermFrequencyEnumerator(this.readers, this.firstDocumentIdForSegments, termToSeek);
        }

        /// <summary>
        /// Gets the term positions enumerator.
        /// </summary>
        /// <param name="termToSeek">The term to seek.</param>
        /// <returns>An enumerator of term positions.</returns>
        public override ITermPositionEnumerator GetTermPositionsEnumerator(Term termToSeek = null)
        {
            return new MultiSegmentTermPositionEnumerator(this.readers, this.firstDocumentIdForSegments, termToSeek);
        }

        /// <summary>
        /// Gets the document terms enumerator.
        /// </summary>
        /// <param name="termToSeek">The term to seek.</param>
        /// <returns>An enumerator of Documents and term frequency.</returns>
        public override ITermEnumerator GetTermEnumerator(Term termToSeek)
        {
            return new MultiSegmentTermEnumerator(this.readers, this.firstDocumentIdForSegments, termToSeek);
        }

        /// <summary>
        /// Indexes the of.
        /// </summary>
        /// <param name="documentId">The document identifier.</param>
        /// <returns>The index</returns>
        private int IndexOf(int documentId)
        {
            int low = 0,
                hi = this.readers.Length;

            while (hi >= low)
            {
                int mid = (low + hi) >> 1;
                int value = this.firstDocumentIdForSegments[mid];
                if (documentId < value)
                    hi = mid - 1;
                else if (documentId > value)
                    low = mid + 1;
                else
                    return mid;
            }

            return hi;
        }
    }
}
