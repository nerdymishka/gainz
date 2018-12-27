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
using System.IO;
using NerdyMishka.Search.IO;
using NerdyMishka.Search.Analysis;

namespace NerdyMishka.Search.Index
{
    /*
    /// <summary>
    /// Class IndexFactory.
    /// </summary>
    public class IndexFactory
    {
        /// <summary>
        /// Gets or sets the analyzer.
        /// </summary>
        /// <value>The analyzer.</value>
        public static IAnalyzer Analyzer { get; set; } 

        /// <summary>
        /// Opens the writer.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="create">if set to <c>true</c> [create].</param>
        /// <returns>IndexWriter.</returns>
        public static IndexWriter OpenWriter(string path, Analysis.IAnalyzer analyzer = null, bool create = false)
        {
            // TODO: replace with standard analyzer
            if (analyzer == null)
                analyzer = Analyzer;

            return new IndexWriter(path, analyzer, create);
        }

        /// <summary>
        /// Opens the writer.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="create">if set to <c>true</c> [create].</param>
        /// <returns>IndexWriter.</returns>
        public static IndexWriter OpenWriter(FileInfo file, Analysis.IAnalyzer analyzer = null, bool create = false)
        {
            // TODO: replace with standard analyzer
            if (analyzer == null)
                analyzer = Analyzer;

            return new IndexWriter(file, analyzer, create);
        }

        /// <summary>
        /// Opens the writer.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="analyzer">The analyzer.</param>
        /// <param name="create">if set to <c>true</c> [create].</param>
        /// <returns>IndexWriter.</returns>
        public static IndexWriter OpenWriter(IDirectory directory, Analysis.IAnalyzer analyzer = null, bool create = false)
        {
            // TODO: replace with standard analzyer
            if (analyzer == null)
                analyzer = Analyzer;

            return new IndexWriter(directory, analyzer, create);
        }

        /// <summary>
        /// Opens the reader.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns>IndexReader.</returns>
        public static IndexReader OpenReader(string path)
        {
            return OpenReader(FileSystemDirectory.GetDirectory(path, false));
        }

        /// <summary>
        /// Opens the reader.
        /// </summary>
        /// <param name="file">The file.</param>
        /// <returns>IndexReader.</returns>
        public static IndexReader OpenReader(FileInfo file)
        {
            return OpenReader(FileSystemDirectory.GetDirectory(file, false));
        }

        /// <summary>
        /// Opens the reader.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <returns>IndexReader.</returns>
        public static IndexReader OpenReader(IDirectory directory)
        {
            lock(directory.SyncLock)
            {
                var segmentInfoList = new SegmentInfoList();
                segmentInfoList.Read(directory);

                if (segmentInfoList.Count == 1)
                    return new SegmentReader(segmentInfoList[0], true);

                var readers = new SegmentReader[segmentInfoList.Count];
                int i = 0, l = readers.Length;
                for (; i < l ; i++)
                {
                    readers[i] = new SegmentReader(segmentInfoList[i], i == segmentInfoList.Count - 1);
                }

                return new SegmentListReader(readers);
            }
        }
    }
    */
}
