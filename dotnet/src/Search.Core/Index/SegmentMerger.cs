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
using System.Collections.Concurrent;
using System.Linq;
using BadMishka.DocumentFormat.LuceneIndex.Store;

namespace BadMishka.DocumentFormat.LuceneIndex.Index
{
    /// <summary>
    /// Class SegmentMerger. This class cannot be inherited.
    /// </summary>
    public sealed class SegmentMerger
    {
        /// <summary>
        /// The directory
        /// </summary>
        private IDirectory directory;

        /// <summary>
        /// The segment name
        /// </summary>
        private string segmentName;

        /// <summary>
        /// The frequency writer
        /// </summary>
        private ILuceneBinaryWriter frequencyWriter = null;

        /// <summary>
        /// The proxy writer
        /// </summary>
        private ILuceneBinaryWriter proxyWriter = null;

        /// <summary>
        /// The term information writer
        /// </summary>
        private TermInfoListWriter termInfosWriter = null;

        /// <summary>
        /// The readers
        /// </summary>
        private ConcurrentBag<SegmentReader> readers = new ConcurrentBag<SegmentReader>();

        /// <summary>
        /// The field information list
        /// </summary>
        private FieldInfoList fieldInfoList;

        /// <summary>
        /// The term information
        /// </summary>
        private TermInfo termInfo = new TermInfo();

        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentMerger"/> class.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="segmentName">Name of the segment.</param>
        public SegmentMerger(IDirectory directory, string segmentName)
        {
            this.directory = directory;
            this.segmentName = segmentName;
        }

        /// <summary>
        /// Gets the <see cref="SegmentReader"/> at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The reader for index</returns>
        internal SegmentReader this[int index]
        {
            get
            {
                return this.readers.ElementAt(index);
            }
        }

        /// <summary>
        /// Adds the reader.
        /// </summary>
        /// <param name="reader">The reader.</param>
        public void Add(SegmentReader reader)
        {
            this.readers.Add(reader);
        }

        /// <summary>
        /// Merges the readers specified by the {@link #add} method into the directory passed to the constructor
        /// </summary>
        public void Merge()
        {
           try
            {
                this.MergeFields();
                this.MergeTerms();
                this.MergeNorms();
            }
            finally
            {
                this.DisposeReaders();
            }
        }

        /// <summary>
        /// Dispose all <see cref="readers"/> that have been added.
        /// Should not be called before merge().
        /// </summary>
        public void DisposeReaders()
        {
            for (int i = 0; i < this.readers.Count; i++)
            {
                // close readers
                var reader = this.readers.ElementAt(i);
                reader.Dispose();
            }
        }

        /// <summary>
        /// Merges the fields.
        /// </summary>
        private void MergeFields()
        {
            this.fieldInfoList = new FieldInfoList(); // merge Field names
            int i = 0,
                l = this.readers.Count;

            for (; i < l; i++)
            {
                var reader = this.readers.ElementAt(i);
                this.fieldInfoList.AddRange(reader.FieldInfoList);
            }

            this.fieldInfoList.Serialize(this.directory, this.segmentName + ".fnm");

            using (var fieldsWriter = new FieldsWriter(this.directory, this.segmentName, this.fieldInfoList))
            {
                i = 0;
                l = this.readers.Count;

                for (; i < l; i++)
                {
                    var reader = this.readers.ElementAt(i);
                    int l2 = reader.TotalDocumentCount;
                    for (int j = 0; j < l2; j++)
                    {
                        if (!reader.IsDocumentDeleted(j))
                        {
                            // skip deleted docs
                            fieldsWriter.AddDocument(reader[j]);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Merges the terms.
        /// </summary>
        private void MergeTerms()
        {
            using (this.frequencyWriter = this.directory.OpenWriteFile(this.segmentName + ".frq"))
            using (this.proxyWriter = this.directory.OpenWriteFile(this.segmentName + ".prx"))
            using (this.termInfosWriter = new TermInfoListWriter(this.directory, this.segmentName, this.fieldInfoList))
            {
                this.MergeTermInfos();
            }
        }

        /// <summary>
        /// Merges the term information.
        /// </summary>
        private void MergeTermInfos()
        {
            using (var queue = new SegmentMergeQueue(this.readers.Count))
            {
                int baseValue = 0,
                    i = 0,
                    l = this.readers.Count;

                for (; i < l; i++)
                {
                    var reader = this.readers.ElementAt(i);
                    var enumerator = reader.GetTermsEnumerator();
                    var segmentMergeInfo = new SegmentMergeInfo(baseValue, enumerator, reader);
                    baseValue += reader.Count;

                    if (segmentMergeInfo.MoveNext())
                        queue.Put(segmentMergeInfo);
                    else
                        segmentMergeInfo.Dispose();
                }

                var match = new SegmentMergeInfo[this.readers.Count];

                while (queue.Count > 0)
                {
                    int matchSize = 0; // pop matching terms
                    match[matchSize++] = queue.Pop();
                    Term term = match[0].Term;
                    var top = queue.Top();

                    while (top != null && term.CompareTo(top.Term) == 0)
                    {
                        match[matchSize++] = queue.Pop();
                        top = queue.Top();
                    }

                    // add new term information.
                    this.MergeTermInfo(match, matchSize);

                    while (matchSize > 0)
                    {
                        SegmentMergeInfo smi = match[--matchSize];
                        if (smi.MoveNext())
                            queue.Put(smi);
                        else
                            smi.Dispose();
                    }
                }
            }
        }

        /// <summary>
        /// Merge one term found in one or more segments. The array <paramref name="segmentMergeInfo"/>
        /// contains segments that are positioned at the same term. <paramref name="count"/>
        /// is the number of cells in the array actually occupied.
        /// </summary>
        /// <param name="segmentMergeInfo">array of segments</param>
        /// <param name="count">number of cells in the array actually occupied</param>
        private void MergeTermInfo(SegmentMergeInfo[] segmentMergeInfo, int count)
        {
            long frequencyPosition = this.frequencyWriter.Position;
            long proxyPosition = this.proxyWriter.Position;
            int docsCountForTerm = this.AppendPostings(segmentMergeInfo, count);

            if (docsCountForTerm > 0)
            {
                // add an entry to the dictionary with Positions to proxy and frequency files
                this.termInfo.Set(docsCountForTerm, frequencyPosition, proxyPosition);
                this.termInfosWriter.Write(segmentMergeInfo[0].Term, this.termInfo);
            }
        }

        /// <summary>
        /// Process postings from multiple segments all positioned on the
        /// same term. Writes out merged entries into <see cref="frequencyWriter"/> and
        /// the <see cref="proxyWriter"/> streams.
        /// </summary>
        /// <param name="segmentMergeInfo">array of segments </param>
        /// <param name="count">The number of cells in the array actually occupied</param>
        /// <returns> 
        ///  The number of documents across all segments where this term was found
        /// </returns>
        private int AppendPostings(SegmentMergeInfo[] segmentMergeInfo, int count)
        {
            int lastDoc = 0;
            int docCountForTerm = 0; 

            for (int i = 0; i < count; i++)
            {
                SegmentMergeInfo smi = segmentMergeInfo[i];
                int baseValue = 0,
                    documentIndex = 0,
                    frequency = 0,
                    documentCode = 0;
                var positions = (SegmentDocumentTermPositionEnumerator)smi.DocumentTermsPositionEnumerator;
                var termInfo = ((SegmentTermFrequencyEnumerator)smi.TermsEnumerator).TermInfo;
                int[] documentIndexMap = smi.DocumentIndexMap;

                baseValue = smi.Index;
                positions.Seek(termInfo);

                while (positions.MoveNext())
                {
                    documentIndex = positions.Current.DocumentId;
                    if (documentIndexMap != null)
                        documentIndex = documentIndexMap[documentIndex]; // map around deletions

                    documentIndex += baseValue; // convert to merged space

                    if (documentIndex < lastDoc)
                        throw new ArgumentOutOfRangeException("documents are out of order");

                    docCountForTerm++;

                    frequency = positions.Current.Frequency;
                    documentCode = (documentIndex - lastDoc) << 1; // use low bit to flag frequency=1
                    lastDoc = documentIndex;

                    if (frequency == 1)
                    {
                        this.frequencyWriter.WriteVariableLengthInt(documentCode | 1); // write doc & frequency=1
                    }
                    else
                    {
                        this.frequencyWriter.WriteVariableLengthInt(documentCode); // write doc
                        this.frequencyWriter.WriteVariableLengthInt(frequency); // write frequency in doc
                    }

                    int lastPosition = 0; // write position deltas
                    for (int j = 0; j < frequency; j++)
                    {
                        int position = positions.ReadNextPosition();
                        this.proxyWriter.WriteVariableLengthInt(position - lastPosition);
                        lastPosition = position;
                    }
                }
            }

            return docCountForTerm;
        }

        /// <summary>
        /// Merges the norms.
        /// </summary>
        private void MergeNorms()
        {
            for (int i = 0; i < this.fieldInfoList.Count; i++)
            {
                FieldInfo fi = this.fieldInfoList[i];
                if (fi.IsIndexed)
                {
                    using (var writer = this.directory.OpenWriteFile(this.segmentName + ".f" + i))
                    { 
                        foreach (var reader in this.readers)
                        {
                            byte[] input = reader.ReadNormalizedBytes(fi.Name);
                            int l = reader.TotalDocumentCount;
                            for (int k = 0; k < l; k++)
                            {
                                byte norm = input != null ? input[k] : (byte)0;
                                if (!reader.IsDocumentDeleted(k))
                                {
                                    writer.Write(norm);
                                }
                            }
                        }
                    }  
                }
            }
        }
    }
}
