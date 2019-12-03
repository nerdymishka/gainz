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
using System.IO;
using System.Linq;
using NerdyMishka.Search.Analysis;
using NerdyMishka.Search.Collections;
using NerdyMishka.Search.Documents;
using NerdyMishka.Search.IO;

namespace NerdyMishka.Search.Index
{
    /// <summary>
    /// Class IndexWriter.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class IndexWriter : IDisposable
    {
        /// <summary>
        /// The synchronize lock
        /// </summary>
        private object syncLock = new object();

        /// <summary>
        /// The directory
        /// </summary>
        private IFileProvider directory;

        /// <summary>
        /// The analyzer
        /// </summary>
        private IAnalyzer analyzer;

        /// <summary>
        /// The ram directory
        /// </summary>
        private IFileProvider ramDirectory = new RamStorage();

        /// <summary>
        /// The segments information list
        /// </summary>
        private SegmentInfoList segmentsInfoList = new SegmentInfoList();
     /* 
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexWriter"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="create">if set to <c>true</c> [create].</param>
        public IndexWriter(string filePath, IAnalyzer analyzer, bool create = true)
           : this(FileSystemDirectory.GetDirectory(filePath, create), analyzer, create)
        {
        }
        
   
        /// <summary>
        /// Initializes a new instance of the <see cref="IndexWriter"/> class.
        /// </summary>
        /// <param name="filePath">The file path.</param>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="create">if set to <c>true</c> [create].</param>
        public IndexWriter(FileInfo filePath, IAnalyzer analyzer, bool create = true)
            : this(FileSystemDirectory.GetDirectory(filePath, create), analyzer, create)
        {
        }
        */

        /// <summary>
        /// Initializes a new instance of the <see cref="IndexWriter"/> class.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="create">if set to <c>true</c> [create].</param>
        public IndexWriter(IFileProvider directory, IAnalyzer analyzer, bool create = false)
        {
            this.directory = directory;
            this.analyzer = analyzer;

            var syncLock = this.directory.GetOrAddSyncLock();
            lock (syncLock)
            {
                if (create)
                    this.segmentsInfoList.Write(directory);
                else
                    this.segmentsInfoList.Read(directory);
            }
        }

        /// <summary>
        /// Gets or sets the similarity.
        /// </summary>
        /// <value>The similarity.</value>
        public Similarity Similarity { get; set; }

        /// <summary>
        /// Gets or sets the information stream.
        /// </summary>
        /// <value>The information stream.</value>
        public System.IO.TextWriter InfoStream { get; set; }

        /// <summary>
        /// Gets or sets the merge factor.
        /// </summary>
        /// <value>The merge factor.</value>
        public int MergeFactor { get; set; } = 10;

        /// <summary>
        /// Gets or sets the minimum merge docs.
        /// </summary>
        /// <value>The minimum merge docs.</value>
        public int MinMergeDocs { get; set; } = 10;

        /// <summary>
        /// Gets or sets the maximum merge docs.
        /// </summary>
        /// <value>The maximum merge docs.</value>
        public int MaxMergeDocs { get; set; } = int.MaxValue;

        /// <summary>
        /// Gets or sets the maximum length of the field.
        /// </summary>
        /// <value>The maximum length of the field.</value>
        public int MaxFieldLength { get; set; } = 10000;

        /// <summary>
        /// Gets the analyzer.
        /// </summary>
        /// <value>The analyzer.</value>
        public IAnalyzer Analyzer => this.analyzer;

        /// <summary>
        /// Gets the document count.
        /// </summary>
        /// <value>The document count.</value>
        public virtual int DocumentCount
        {
            get
            {
                lock (this.syncLock)
                {
                    return this.segmentsInfoList.Sum(o => o.DocumentCount);   
                }
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.FlushRamSegments();
            this.ramDirectory.Dispose();
            this.directory.Dispose();
        }

        /// <summary>
        /// Adds the specified document.
        /// </summary>
        /// <param name="document">The document.</param>
        /// <param name="analyzer">The analyzer.</param>
        public void Add(Document document, IAnalyzer analyzer)
        {
            var documentWriter = new DocumentWriter(this.ramDirectory, this.analyzer, this.Similarity, this.MaxFieldLength);
            var segmentName = this.GenerateSegmentName();
            documentWriter.Write(document, segmentName);

            lock (this.syncLock)
            {
                this.segmentsInfoList.Add(new SegmentInfo(segmentName, 1, this.ramDirectory));
                this.MaybeMergeSegments();
            }
        }

        /// <summary>
        /// Adds the range.
        /// </summary>
        /// <param name="directories">The directories.</param>
        public void AddRange(IFileProvider[] directories)
        {
            this.Optimize();
            int minSegment = this.segmentsInfoList.Count,
                segmentsAddedSinceMerge = 0,
                i = 0,
                l = directories.Length;

            for (; i < l; i++)
            {
                var list = new SegmentInfoList();
                list.Read(directories[i]);

                for (int j = 0; i < list.Count; j++)
                {
                    this.segmentsInfoList.Add(list[i]);
                    if (++segmentsAddedSinceMerge == this.MergeFactor)
                    {
                        this.MergeSegments(minSegment, false);
                        segmentsAddedSinceMerge = 0;
                    }
                }
            }

            this.Optimize();
        }

        /// <summary>
        /// Optimizes this instance.
        /// </summary>
        public void Optimize()
        {
            this.FlushRamSegments();

            int count = this.segmentsInfoList.Count;

            while (count > 1 || (count == 1 && SegmentReader.HasDeletionsForSegment(this.segmentsInfoList[0])))
            {
                int minSegment = count - this.MergeFactor;
                this.MergeSegments(minSegment < 0 ? 0 : minSegment);
                count = this.segmentsInfoList.Count;
            }
        }

        /// <summary>
        /// Incremental segment merger.
        /// </summary>
        private void MaybeMergeSegments()
        {
            long targetMergeDocs = this.MinMergeDocs;
            while (targetMergeDocs <= this.MaxMergeDocs)
            {
                // find segments smaller than current target size
                int minSegment = this.segmentsInfoList.Count;
                int mergeDocs = 0;
                while (--minSegment >= 0)
                {
                    SegmentInfo si = this.segmentsInfoList[minSegment];
                    if (si.DocumentCount >= targetMergeDocs)
                        break;
                    mergeDocs += si.DocumentCount;
                }

                if (mergeDocs >= targetMergeDocs)
                    this.MergeSegments(minSegment + 1);
                else
                    break;

                targetMergeDocs *= this.MergeFactor; // increase target size
            }
        }

        /// <summary>
        /// Merges the segments.
        /// </summary>
        /// <param name="minSegment">The minimum segment.</param>
        private void MergeSegments(int minSegment)
        {
            this.MergeSegments(minSegment, true);
        }

        /// <summary>
        /// Writes the line.
        /// </summary>
        /// <param name="message">The message.</param>
        private void WriteLine(string message)
        {
            if (this.InfoStream != null)
                this.InfoStream.WriteLine(message);
        }

        /// <summary>
        /// Generates the name of the segment.
        /// </summary>
        /// <returns>The new segment name</returns>
        private string GenerateSegmentName()
        {
            lock (this.syncLock)
            {
                return "_" + (this.segmentsInfoList.Counter++).EncodeToString();
            }
        }

        /// <summary>
        /// Deletes the segments.
        /// </summary>
        /// <param name="segmentsToDelete">The segments to delete.</param>
        private void DeleteSegments(IList<SegmentReader> segmentsToDelete)
        {
            int i = 0,
               l = segmentsToDelete.Count;
            var deletable = new ThreadSafeList<string>();

            this.DeleteFiles(this.ReadFilesForDeletion(), deletable);

            for (; i < l; i++)
            { 
                var reader = segmentsToDelete[i];

                if (reader.Directory == this.directory)
                    this.DeleteFiles(reader.Files, deletable);
                else
                    this.DeleteFiles(reader.Files, reader.Directory);
            }

            this.WriteFilesForDeletion(deletable);            
        }

        /// <summary>
        /// Deletes the files.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="directory">The directory.</param>
        private void DeleteFiles(IList<string> files, IFileProvider directory)
        {
            int i = 0,
                l = files.Count;

            for (; i < l; i++)
            {
                directory.Delete(files[i]);
            }
        }

        /// <summary>
        /// Deletes the files.
        /// </summary>
        /// <param name="files">The files.</param>
        /// <param name="filesToDelete">The files to delete.</param>
        private void DeleteFiles(IList<string> files, ThreadSafeList<string> filesToDelete)
        {
            int i = 0,
                l = files.Count;

            for (; i < l; i++)
            {
                var file = files[i];
                try
                {
                    this.directory.Delete(file);
                }
                catch (IOException ex)
                {
                    if (this.directory.Exists(file))
                    {
                        this.WriteLine($"exception: file delete failed for {file}");
                        this.WriteLine($"exception: {ex.Message}");

                        filesToDelete.Add(file);
                    }
                }
            }
        }

        /// <summary>
        /// Flushes the ram segments.
        /// </summary>
        private void FlushRamSegments()
        {
            SegmentInfo si = null,
               lastSi = null;

            int minSegment = this.segmentsInfoList.Count - 1,
                documentCount = 0,
                nextCount = 0;

            while (minSegment >= 0 && this.segmentsInfoList[minSegment].Directory == this.ramDirectory)
            {
                documentCount += this.segmentsInfoList[minSegment].DocumentCount;
                minSegment--;
            }

            si = this.segmentsInfoList[minSegment];
            lastSi = this.segmentsInfoList[this.segmentsInfoList.Count - 1];
            nextCount = documentCount + si.DocumentCount;

            if (minSegment < 0 || nextCount > this.MergeFactor || !(lastSi.Directory == this.ramDirectory))
                minSegment++;

            if (minSegment >= this.segmentsInfoList.Count)
                return;                   // none to merge

            this.MergeSegments(minSegment);
        }

        /// <summary>
        /// Merges the segments.
        /// </summary>
        /// <param name="minSegment">The minimum segment.</param>
        /// <param name="delete">if set to <c>true</c> [delete].</param>
        private void MergeSegments(int minSegment, bool delete)
        {
            string mergedName = this.GenerateSegmentName();
            int mergedDocCount = 0;

            this.WriteLine("merging segments");

            var merger = new SegmentMerger(this.directory, mergedName);
            var segmentsToDelete = new ThreadSafeList<SegmentReader>();

            for (int i = minSegment; i < this.segmentsInfoList.Count; i++)
            {
                var si = this.segmentsInfoList[i];

                this.WriteLine($" {si.Name} ({si.DocumentCount} docs)");

                var reader = new SegmentReader(si);
                merger.Add(reader);
                if (delete)
                    segmentsToDelete.Add(reader);     // queue for deletion
                mergedDocCount += si.DocumentCount;
            }

            this.WriteLine(string.Empty);
            this.WriteLine($" into {mergedName} ({mergedDocCount} docs)");

            merger.Merge();

            // (╯°□°）╯︵ ┻━┻  JavaScript array pop.
            this.segmentsInfoList.Count = minSegment;
            this.segmentsInfoList.Add(new SegmentInfo(mergedName, mergedDocCount, this.directory));

            var syncLock = this.directory.GetOrAddSyncLock();
            lock (syncLock)
            {
                this.segmentsInfoList.Write(this.directory);
                this.DeleteSegments(segmentsToDelete);
            }
        }

        /// <summary>
        /// Reads the files for deletion.
        /// </summary>
        /// <returns>list of files to delete.</returns>
        private IList<string> ReadFilesForDeletion()
        {
            var list = new List<string>();
            if (!this.directory.Exists("deletable"))
                return list;

            using (var reader = this.directory.OpenReader("deletable"))
            {
                for (int i = reader.ReadInt32(); i > 0; i--)
                {
                    list.Add(reader.ReadString());
                }
            }

            return list;
        }

        /// <summary>
        /// Writes files that are up for deletion.
        /// </summary>
        /// <param name="files">The files.</param>
        private void WriteFilesForDeletion(IList<string> files)
        {
#pragma warning disable SA1650
            using (var writer = this.directory.OpenWriter("deletable.new"))
            {
                writer.Write(files.Count);
                int i = 0,
                    l = files.Count;

                for (; i < l; i++)
                {
                    writer.Write(files[i]);
                }
            }

            this.directory.Move("deletable.new", "deletable");
#pragma warning restore SA1650
        }    
    }
}
