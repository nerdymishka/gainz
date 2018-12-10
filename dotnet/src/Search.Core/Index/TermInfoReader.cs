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
using System.Threading;
using NerdyMishka.Search.IO;

namespace NerdyMishka.Search.Index
{
    /// <summary>
    /// Class <see cref="TermInfoListReader"/> reads tuples of <see cref="Term"/> and <see cref="TermInfo"/> 
    /// in a <see cref="IDirectory"/>. Pairs are accessed by querying by the ordinal position or 
    /// a given <see cref="Term"/>  This class cannot be inherited.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    internal sealed class TermInfoReader : IDisposable
    {
        /// <summary>
        /// The number of elements in the arrays
        /// </summary>
        private long count;

        /// <summary>
        /// The directory
        /// </summary>
        private IFileProvider directory;

        /// <summary>
        /// <see cref="ThreadLocal{T}"/> ensures there is only one instance of an object per thread.
        /// </summary>
        private ThreadLocal<SegmentTermFrequencyEnumerator> localThreadSlot = 
            new ThreadLocal<SegmentTermFrequencyEnumerator>();

        /// <summary>
        /// The name of the segment.
        /// </summary>
        private string segmentName;
       
        /// <summary>
        /// Initializes a new instance of the <see cref="TermInfoListReader"/> class.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="segmentName">Name of the segment.</param>
        /// <param name="fieldInfoList">The field information list.</param>
        public TermReader(IFileProvider directory, string segmentName, FieldInfoList fieldInfoList)
        {
            this.directory = directory;
            this.segmentName = segmentName;
            this.FieldInfoList = fieldInfoList;

            var enumerator = this.GetThreadLocalSegmentTermFrequencyEnumerator();

            this.count = enumerator.Count;
            this.ReadIndex();
        }

        /// <summary>
        /// Gets or sets the field information list.
        /// </summary>
        /// <value>The field information list.</value>
        public FieldInfoList FieldInfoList { get; set; }

        /// <summary>
        /// Gets the index pointers.
        /// </summary>
        /// <value>The index pointers.</value>
        public long[] IndexPointers { get; private set; }

        /// <summary>
        /// Gets the terms.
        /// </summary>
        /// <value>The terms.</value>
        public Term[] Terms { get; private set; }

        /// <summary>
        /// Gets the term information.
        /// </summary>
        /// <value>An array of term information.</value>
        public TermInfo[] TermInfos { get; private set; }

        /// <summary>
        /// Gets the number of <see cref="Term"/>/<see cref="TermInfo"/> pairs 
        /// </summary>
        /// <value>The count.</value>
        internal long Count => this.count;

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.localThreadSlot != null)
            {
                this.localThreadSlot.Value = null;
                this.localThreadSlot.Dispose();
                this.localThreadSlot = null;
            }
        }

        /// <summary>
        /// Gets the segment term frequency enumerator.
        /// </summary>
        /// <returns>SegmentTermFrequencyEnumerator.</returns>
        public SegmentTermFrequencyEnumerator GetSegmentTermFrequencyEnumerator()
        {
            var enumerator = this.GetThreadLocalSegmentTermFrequencyEnumerator();
            if (enumerator.Position != -1)
                this.SeekEnum(0);

            return (SegmentTermFrequencyEnumerator)enumerator.Clone();
        }

        public SegmentTermFrequencyEnumerator GetSegmentTermFrequencyEnumerator(Term termToSeek)
        {
            var enumerator = this.GetSegmentTermFrequencyEnumerator();
            this.Read(termToSeek);

            return (SegmentTermFrequencyEnumerator)enumerator.Clone();
        }

        /// <summary>
        /// Reads the specified position.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>Term.</returns>
        public Term Read(int position)
        {
            if (this.count == 0)
                return null;

            var enumerator = this.GetThreadLocalSegmentTermFrequencyEnumerator();
            Term currentTerm = null;
            if (enumerator != null && enumerator.Current != null)
                currentTerm = enumerator.Current.Term;

            if(currentTerm != null && position >= enumerator.Position &&
                position < (enumerator.Position + TermInfoWriter.IndexInterval))
            {
                return this.ScanEnum(position);
            }

            this.SeekEnum(position / TermInfoWriter.IndexInterval);

            return this.ScanEnum(position);
        }

        /// <summary>
        /// Reads the <see cref="TermInfo"/> for the specified <see cref="Term"/> in the set if found; 
        /// otherwise, null. 
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns>a <see cref="TermInfo"/> object. </returns>
        public TermInfo Read(Term term)
        {
            if (this.count == 0)
                return null;

            // optimize sequential access: first try scanning cached enum w/o seeking
            var enumerator = this.GetThreadLocalSegmentTermFrequencyEnumerator();
            Term currentTerm = enumerator.Current.Term;
            Term previousTerm = enumerator.PreviousTerm;

            if (currentTerm != null && 
                ((previousTerm != null && term.CompareTo(previousTerm) > 0) || 
                term.CompareTo(currentTerm) >= 0))
            {
                int enumOffset = (int)(enumerator.Position / TermInfoWriter.IndexInterval) + 1;
                if (this.Terms.Length == enumOffset || term.CompareTo(this.Terms[enumOffset]) < 0)
                    return this.ScanEnum(term); // no need to seek*/
            }

            // random-access: must seek
            this.SeekEnum(this.GetIndexOffset(term));

            return this.ScanEnum(term);
        }

        /// <summary>
        /// Gets the index of the given <see cref="Term"/>.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns>The index.</returns>
        public long IndexOf(Term term)
        {
            if (this.count == 0)
                return -1;

            int indexOffset = this.GetIndexOffset(term);
            this.SeekEnum(indexOffset);

            var enumerator = this.GetThreadLocalSegmentTermFrequencyEnumerator();
            while (term.CompareTo(enumerator.Current) > 0 && enumerator.MoveNext())
            {
            }

            if (term.CompareTo(enumerator.Current) == 0)
                return enumerator.Position;
            else
                return -1;
        }

        /// <summary>
        /// Gets the segment term frequency enumerator for the current thread.
        /// </summary>
        /// <returns>An enumerator for the current thread</returns>
        private SegmentTermFrequencyEnumerator GetThreadLocalSegmentTermFrequencyEnumerator()
        {
            SegmentTermFrequencyEnumerator enumerator = null;
            if (!this.localThreadSlot.IsValueCreated)
            {
                enumerator = new SegmentTermFrequencyEnumerator(this.directory.OpenReader(this.segmentName + ".tii"), this.FieldInfoList, true);
                this.localThreadSlot.Value = enumerator;
            }
            else
            {
                enumerator = this.localThreadSlot.Value;
            }

            return enumerator;
        }

        /// <summary>
        /// Reads the index.
        /// </summary>
        private void ReadIndex()
        {
            using (var indexEnumerator = 
                new SegmentTermFrequencyEnumerator(this.directory.OpenReader(this.segmentName + ".tii"), this.FieldInfoList, true))
            {
                int length = (int)indexEnumerator.Count;

                this.Terms = new Term[length];
                this.TermInfos = new TermInfo[length];
                this.IndexPointers = new long[length];

                for (int i = 0; indexEnumerator.MoveNext(); i++)
                {
                    this.Terms[i] = indexEnumerator.Current.Term;
                    this.TermInfos[i] = indexEnumerator.TermInfo;
                    this.IndexPointers[i] = indexEnumerator.IndexPointer;
                }
            }
        }

        /// <summary>
        /// Returns the offset of the greatest index entry which is less than or equal to term.
        /// </summary>
        /// <param name="term">The term used to find the offset.</param>
        /// <returns>The index offset.</returns>
        private int GetIndexOffset(Term term)
        {
            int lo = 0; // binary search indexTerms[]
            int hi = this.Terms.Length - 1;

            while (hi >= lo)
            {
                int mid = (lo + hi) >> 1;
                int delta = term.CompareTo(this.Terms[mid]);
                if (delta < 0)
                    hi = mid - 1;
                else if (delta > 0)
                    lo = mid + 1;
                else
                    return mid;
            }

            return hi;
        }

        /// <summary>
        /// Seeks the enum.
        /// </summary>
        /// <param name="indexOffset">The index offset.</param>
        private void SeekEnum(int indexOffset)
        {
            var enumerator = this.GetThreadLocalSegmentTermFrequencyEnumerator();

            enumerator.Seek(
                this.IndexPointers[indexOffset],
                (indexOffset * TermInfoWriter.IndexInterval) - 1,
                this.Terms[indexOffset],
                this.TermInfos[indexOffset]);
        }

        /// <summary>
        /// Scans the enum.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <returns>a <see cref="TermInfo"/> object</returns>
        private TermInfo ScanEnum(Term term)
        {
            var enumerator = this.GetThreadLocalSegmentTermFrequencyEnumerator();
            Term nextTerm = null;

            while (term.CompareTo(nextTerm = enumerator.Current.Term) > 0 && enumerator.MoveNext())
            {
            }

            if (nextTerm != null && term.CompareTo(nextTerm) == 0)
                return enumerator.TermInfo;
            else
                return null;
        }

        /// <summary>
        /// Scans the enum.
        /// </summary>
        /// <param name="position">The position.</param>
        /// <returns>A <see cref="Term"/></returns>
        private Term ScanEnum(int position)
        {
            var enumerator = this.GetThreadLocalSegmentTermFrequencyEnumerator();
            while (enumerator.Position < position)
                if (!enumerator.MoveNext())
                    return null;

            return enumerator.Current.Term;
        }
    }
}
