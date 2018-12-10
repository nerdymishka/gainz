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
using System.Collections.Generic;
using NerdyMishka.Search.Documents;
using NerdyMishka.Search.IO;

namespace NerdyMishka.Search.Index
{
    /// <summary>
    /// Class SegmentReader.
    /// </summary>
    /// <seealso cref="BadMishka.DocumentFormat.LuceneIndex.Index.IndexReader" />
    public class SegmentReader : IndexReader
    {
        /// <summary>
        /// The synchronize root
        /// </summary>
        private readonly object syncRoot = new object();

        /// <summary>
        /// The directory
        /// </summary>
        private IFileProvider directory;

        /// <summary>
        /// The close directory
        /// </summary>
        private bool closeDirectory = false;

        /// <summary>
        /// The segment
        /// </summary>
        private string segmentName;

        /// <summary>
        /// The field information list
        /// </summary>
        private FieldInfoList fieldInfoList;

        /// <summary>
        /// The field reader
        /// </summary>
        private FieldReader fieldReader;

        /// <summary>
        /// The term information reader
        /// </summary>
        private TermInfoListReader termInfoReader;

        /// <summary>
        /// The deleted docs
        /// </summary>
        private BitVector deletedDocs = null;

        /// <summary>
        /// The deleted docs changed
        /// </summary>
        private bool deletedDocsChanged = false;

        /// <summary>
        /// The frequency reader
        /// </summary>
        private IBinaryReader frequencyReader;

        /// <summary>
        /// The proxy reader
        /// </summary>
        private IBinaryReader proxyReader;

        /// <summary>
        /// The normalized factors
        /// </summary>
        private Dictionary<string, Norm> normalizedFactors = new Dictionary<string, Norm>();

        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentReader" /> class.
        /// </summary>
        /// <param name="segmentInfo">The segment information.</param>
        /// <param name="closeDirectory">if set to <c>true</c> [close directory].</param>
        public SegmentReader(SegmentInfo segmentInfo, bool closeDirectory = false)
        {
            this.directory = segmentInfo.Directory;
            this.segmentName = segmentInfo.Name;
            this.closeDirectory = closeDirectory;

            this.fieldInfoList = new FieldInfoList();
            this.fieldInfoList.Deserialize(this.directory, this.segmentName);
            this.fieldReader = new FieldReader(this.directory, this.segmentName, this.fieldInfoList);
            this.termInfoReader = new TermInfoReader(this.directory, this.segmentName, this.fieldInfoList);

            if (HasDeletionsForSegment(segmentInfo))
            {
                this.deletedDocs = this.directory.ReadVector($"{this.segmentName}.del");
            }

            // Obligatory What's The Frequency, Kenneth: https://www.youtube.com/watch?v=jWkMhCLkVOg
            this.frequencyReader = this.directory.OpenReader($"{this.segmentName}.frq");
            this.proxyReader = this.directory.OpenReader($"{this.segmentName}.prx");

            // ಠ‿ಠ Never leave a meme or pop-culture reference behind.
            this.WelcomeNormies();
        }

        /// <summary>
        /// Gets the files.
        /// </summary>
        /// <value>The files.</value>
        public IList<string> Files
        {
            get
            {
                var list = new List<string>(16);
                list.Add(this.segmentName + ".fnm");
                list.Add(this.segmentName + ".fdx");
                list.Add(this.segmentName + ".fdt");
                list.Add(this.segmentName + ".tii");
                list.Add(this.segmentName + ".tis");
                list.Add(this.segmentName + ".frq");
                list.Add(this.segmentName + ".prx");

                if (this.directory.Exists(this.segmentName + ".del"))
                    list.Add(this.segmentName + ".del");

                int i = 0,
                    l = this.fieldInfoList.Count;

                for (; i < l; i++)
                {
                    var fi = this.fieldInfoList[i];
                    if (fi.IsIndexed)
                        list.Add(this.segmentName + ".f" + i);
                }

                return list;
            }
        }

        /// <summary>
        /// The number of active documents
        /// </summary>
        /// <value>The count.</value>
        public override int Count
        {
            get
            {
                int count = this.TotalDocumentCount;
                if (this.deletedDocs != null)
                    count -= this.deletedDocs.Length;

                return count;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not this reader has deletions.
        /// </summary>
        /// <value>The has deletions.</value>
        public bool HasDeletions
        {
            get
            {
                return this.deletedDocs != null && this.deletedDocs.Length > 0;
            }
        }

        /// <summary>
        /// Gets the directory.
        /// </summary>
        /// <value>The directory.</value>
        public IFileProvider Directory => this.directory;

        /// <summary>
        /// Gets the total document count.
        /// </summary>
        /// <value>The total document count.</value>
        public override int TotalDocumentCount => this.fieldReader.Length;

        /// <summary>
        /// Gets the deleted docs.
        /// </summary>
        /// <value>The deleted docs.</value>
        internal BitVector DeletedDocs => this.deletedDocs;

        /// <summary>
        /// Gets the field information list.
        /// </summary>
        /// <value>The field information list.</value>
        internal FieldInfoList FieldInfoList => this.fieldInfoList;

        /// <summary>
        /// Gets the frequency reader.
        /// </summary>
        /// <value>The frequency reader.</value>
        internal IBinaryReader FrequencyReader => this.frequencyReader;

        /// <summary>
        /// Gets the proxy reader.
        /// </summary>
        /// <value>The proxy reader.</value>
        internal IBinaryReader ProxyReader => this.proxyReader;

        /// <summary>
        /// Gets the <see cref="Document"/> with the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns>a <see cref="Document"/>.</returns>
        /// <exception cref="System.ArgumentException">Access to the deleted document is not permitted.</exception>
        public override Document this[int id]
        {
            get
            {
                lock (this.syncRoot)
                {
                    if (this.deletedDocs != null && this.deletedDocs.Get(id))
                        throw new ArgumentException("Access to a deleted document is not permitted");

                    return this.fieldReader.Read(id);
                }
            }
        }

        /// <summary>
        /// Determines whether [has deletions for segment] [the specified segment].
        /// </summary>
        /// <param name="segment">The segment.</param>
        /// <returns><c>true</c> if [has deletions for segment] [the specified segment]; otherwise, <c>false</c>.</returns>
        public static bool HasDeletionsForSegment(SegmentInfo segment)
        {
            return segment.Directory.Exists(segment.Name + ".del");
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        public override void Delete(int id)
        {
            if (this.deletedDocs == null)
                this.deletedDocs = new BitVector(this.TotalDocumentCount);

            this.deletedDocsChanged = true;
            this.deletedDocs.Set(id);
        }

        /// <summary>
        /// Gets the frequency the term appears for a document.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns>The document frequency.</returns>
        public override int GetDocumentFrequency(Term term)
        {
            var termInfo = this.termInfoReader.Read(term);
            if (termInfo == null)
                return 0;

            return termInfo.DocumentFrequency;
        }

        /// <summary>
        /// Gets the terms enumerator.
        /// </summary>
        /// <param name="termToSeek">The term to seek.</param>
        /// <returns>An enumerator for term frequency pairs.</returns>
        public override ITermFrequencyEnumerator GetTermFrequencyEnumerator(Term termToSeek = null)
        {
            if (termToSeek == null)
                return this.termInfoReader.GetSegmentTermFrequencyEnumerator();

            return this.termInfoReader.GetSegmentTermFrequencyEnumerator(termToSeek);
        }

        /// <summary>
        /// Determines whether [is document deleted] [the specified index].
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns><c>true</c> if [is document deleted] [the specified index]; otherwise, <c>false</c>.</returns>
        public override bool IsDocumentDeleted(int index)
        {
            lock (this.syncRoot)
            {
                if (this.deletedDocs == null)
                    return false;

                return this.deletedDocs.Get(index);
            }
        }

        /// <summary>
        /// Gets the document terms enumerator.
        /// </summary>
        /// <param name="termToSeek">The term to seek.</param>
        /// <returns>An enumerator for <see cref="DocumentFrequencyPair"/></returns>
        public override ITermEnumerator GetTermEnumerator(Term termToSeek = null)
        {
            var termInfo = this.termInfoReader.Read(termToSeek);
            if (termInfo != null)
                return new SegmentTermEnumerator(this, termInfo);

            return null;
        }

        /// <summary>
        /// Gets the term positions enumerator.
        /// </summary>
        /// <param name="termToSeek">The term to seek.</param>
        /// <returns>An enumerator for term positions in a document.</returns>
        public override ITermPositionEnumerator GetTermPositionsEnumerator(Term termToSeek)
        {
            var termInfo = this.termInfoReader.Read(termToSeek);
            if (termInfo != null)
                return new SegmentTermPositionEnumerator(this, termInfo);

            return null;
        }

        /// <summary>
        /// Reads the normalized bytes.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>The normalized bytes. </returns>
        public override byte[] ReadNormalizedBytes(string fieldName)
        {
            if (!this.normalizedFactors.ContainsKey(fieldName))
                return null;

            var norm = this.normalizedFactors[fieldName];
            if (norm.Bytes == null)
            {
                var bytes = new byte[this.TotalDocumentCount];
                this.ReadNormalizedBytes(fieldName, bytes);
                norm.Bytes = bytes;
            }

            return norm.Bytes;
        }

        /// <summary>
        /// Reads the bytes from field.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <param name="bytes">The bytes.</param>
        /// <param name="offset">The offset.</param>
        internal void ReadNormalizedBytes(string fieldName, byte[] bytes, int offset = 0)
        {
            var reader = this.OpenNormalizedFactorReader(fieldName);
            if (reader == null)
                return;

            using (reader)
            {
                reader.Read(bytes, offset, bytes.Length);
            }
        }

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.deletedDocsChanged)
                {
                    var syncLock = this.directory.GetOrAddSyncLock();
                    lock (syncLock)
                    {
                        this.directory.WriteVector(this.segmentName + ".tmp", this.deletedDocs);
                        this.directory.Move(this.segmentName + ".tmp", this.segmentName + ".del", true);
                    }

                    this.deletedDocsChanged = false;
                }
            }

            this.fieldReader.Dispose();
            this.termInfoReader.Dispose();

            if (this.frequencyReader != null)
                this.frequencyReader.Dispose();

            if (this.proxyReader != null)
                this.proxyReader.Dispose();

            this.DisposeNorms();

            if (this.closeDirectory && this.directory != null)
                this.directory.Dispose();

            base.Dispose(disposing);
        }

        /// <summary>
        /// Disposes the norms.
        /// </summary>
        private void DisposeNorms()
        {
            lock (this.syncRoot)
            {
                foreach (var norm in this.normalizedFactors.Values)
                {
                    norm.Reader.Dispose();
                }
            }
        }

        /// <summary>
        /// Opens the norms.
        /// </summary>
        private void WelcomeNormies()
        {
            lock (this.syncRoot)
            {
                int i = 0,
                    l = this.fieldInfoList.Count;

                for (; i < l; i++)
                {
                    var fi = this.fieldInfoList[i];
                    if (fi.IsIndexed)
                    {
                        var segmentName = this.segmentName + ".f" + fi.Index;
                        var norm = new Norm(this.directory.OpenReader(this.segmentName));
                        this.normalizedFactors.Add(fi.Name, norm);
                    }
                }
            }
        }

        /// <summary>
        /// Opens the normalized factor reader.
        /// </summary>
        /// <param name="fieldName">Name of the field.</param>
        /// <returns>a reader.</returns>
        private IBinaryReader OpenNormalizedFactorReader(string fieldName)
        {
            if (!this.normalizedFactors.ContainsKey(fieldName))
                return null;

            var norm = this.normalizedFactors[fieldName];
            var reader = (IBinaryReader)norm.Reader.Clone();
            reader.Seek(0);

            return reader;
        }

        /// <summary>
        /// Class Norm.
        /// </summary>
        private class Norm
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="Norm"/> class.
            /// </summary>
            /// <param name="reader">The reader.</param>
            public Norm(IBinaryReader reader)
            {
                this.Reader = reader;
            }

            /// <summary>
            /// Gets the reader.
            /// </summary>
            /// <value>The reader.</value>
            public IBinaryReader Reader { get; private set; }

            /// <summary>
            /// Gets or sets the bytes.
            /// </summary>
            /// <value>The bytes.</value>
            public byte[] Bytes { get; set; }
        }
    }
}
