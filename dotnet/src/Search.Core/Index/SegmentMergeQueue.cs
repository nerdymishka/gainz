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
using NerdyMishka.Search.Collections;
using NerdyMishka.Search.IO;

namespace  NerdyMishka.Search.Index
{
    /// <summary>
    /// Class SegmentMergeQueue. This class cannot be inherited.
    /// </summary>
    /// <seealso cref="BadMishka.DocumentFormat.LuceneIndex.Util.PriorityQueue{BadMishka.DocumentFormat.LuceneIndex.Index.SegmentMergeInfo}" />
    /// <seealso cref="System.IDisposable" />
    internal sealed class SegmentMergeQueue : PriorityQueue<SegmentMergeInfo>, IDisposable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentMergeQueue"/> class.
        /// </summary>
        /// <param name="capacity">The capacity.</param>
        internal SegmentMergeQueue(int capacity)
        {
            this.Initialize(capacity);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if(disposing)
            {
                while (this.Top() != null)
                    this.Pop().Dispose();
            }
        }

        ~SegmentMergeQueue()
        {
            this.Dispose(false);
        }
    }
}
