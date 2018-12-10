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
using NerdyMishka.Search.Documents;

namespace NerdyMishka.Search.Index
{
    /// <summary>
    /// IndexReader is an abstract class, providing an interface for accessing an
    /// index.  This abstract interface enables searching over an index. Any subclass which 
    /// implements <see cref="IndexReader"/> is searchable.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///      For efficiency, in this API documents are often  to via
    ///      <it>document numbers</it>, non-negative integers which each name a unique
    ///      document in the index.
    ///     </para>
    ///     <para>
    ///       These document numbers are ephemeral--they may change
    ///       as documents are added to and deleted from an index.  Clients should thus not
    ///       rely on a given document having the same number between sessions.
    ///     </para>
    /// </remarks>
    /// <seealso cref="System.IDisposable" />
    public abstract class IndexReader : IDisposable
    {
        /// <summary>
        /// The disposed value
        /// </summary>
        private bool disposedValue = false;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexReader"/> class.
        /// </summary>
        protected IndexReader()
        {
        }

        /// <summary>
        /// Finalizes an instance of the <see cref="IndexReader"/> class.
        /// </summary>
        ~IndexReader()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// Gets the number of active documents
        /// </summary>
        public abstract int Count { get; }

        /// <summary>
        /// Gets the total number of documents, including documents waiting to be deleted.
        /// </summary>
        public abstract int TotalDocumentCount { get; }

        /// <summary>
        /// Gets or sets the <see cref="Document"/> with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>A <see cref="Document"/></returns>
        public abstract Document this[int id] { get; }

        /// <summary>
        /// Reads the byte-encoded normalization factor for the named field of
        /// every document.This is used by the search code to score documents.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>Normalized bytes</returns>
        public abstract byte[] ReadNormalizedBytes(string fieldName);

        /// <summary>
        /// Gets the the number of documents containing the term.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns>The number of documents where the term appears.</returns>
        public abstract int GetDocumentFrequency(Term term);

        /// <summary>
        /// Returns an enumeration of all the terms in the index.
        /// The enumeration is ordered by <see cref="Term.CompareTo(object)"/>.  Each term
        /// is greater than all that precede it in the enumeration.
        /// </summary>
        /// <remarks><para>This is <c>terms()</c> in Java</para></remarks>
        /// <param name="termToSeek">The term to seek.</param>
        /// <returns>An enumerator.</returns>
        public abstract ITermFrequencyEnumerator GetTermsEnumerator(Term termToSeek = null);

        /// <summary>
        /// Deletes the document with the integer identifier.  Once a document is
        /// deleted it will not appear in the <see cref="IDocumentTermEnumerator"/> and
        /// <see cref="IDocumentTermPositionEnumerator"/> enumerators.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///          Attempts to read its field with the <see cref="Document"/>
        ///          method will result in an error.The presence of this document may still be
        ///          reflected in the <see cref="DocumentFrequencyPair.Frequency"/> statistic, though
        ///          this will be corrected eventually as the index is further modified.       
        ///     </para>
        /// </remarks>
        /// <param name="id">The identifier.</param>
        public abstract void Delete(int id);

        /// <summary>
        /// Deletes all documents containing <code>term</code>.
        /// This is useful if one uses a document field to hold a unique ID string for
        /// the document.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///      To delete a document with an Id field, construct a
        ///      term with the appropriate field and the unique ID string as its text and
        ///      pass it to this method.
        ///     </para>
        /// </remarks>
        /// <param name="term">The term.</param>
        /// <returns>The delete count.</returns>
        public int Delete(Term term)
        {
            var enumerator = this.GetTermsEnumerator(term);
            if (enumerator == null)
                return 0;

            int deleteCount = 0;
            using (enumerator)
            {
                while (enumerator.MoveNext())
                {
                    this.Delete(enumerator.Current.DocumentId);
                    deleteCount++;
                }
            }

            return deleteCount;
        }

        /// <summary>
        /// Determines if a document with the given identifier has been deleted or not. 
        /// </summary>
        /// <param name="id">The index.</param>
        /// <returns><c>true</c> if [is document deleted] [the specified index]; otherwise, <c>false</c>.</returns>
        public abstract bool IsDocumentDeleted(int id);

        /// <summary>
        /// Returns an enumeration of all the documents which contains the <paramref name="termToSeek"/> 
        /// with the frequency the term appears.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///          For each document, the document number, the frequency of
        ///          the term in that document is also provided, for use in search scoring.
        ///     </para>
        ///     <para>
        ///         The enumeration is ordered by document number.  Each document number
        ///         is greater than all that precede it in the enumeration.
        ///     </para>
        /// </remarks>
        /// <param name="termToSeek">The term to seek.</param>
        /// <returns>An enumerator.</returns>
        public abstract ITermEnumerator GetTermEnumerator(Term termToSeek);

        /// <summary>
        /// Returns an enumeration of all the documents which contains the <paramref name="termToSeek"/>
        /// and the positions of the terms within the document. 
        /// </summary>
        /// <remarks>
        ///     <para>
        ///       For each document, in addition to the document number
        ///       and frequency of the term in that document, a list of all of the ordinal
        ///       positions of the term in the document is available.
        ///     </para>
        /// </remarks>
        /// <param name="termToSeek">The term to seek.</param>
        /// <returns>An enumerator.</returns>
        public abstract ITermPositionEnumerator GetTermPositionsEnumerator(Term termToSeek = null);

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public virtual void Dispose()
        {
            this.Dispose(true); 
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.
                this.disposedValue = true;
            }
        }
    }
}