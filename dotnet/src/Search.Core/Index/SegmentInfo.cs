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
using NerdyMishka.Search.IO;

namespace NerdyMishka.Search.Index
{
    /// <summary>
    /// Class SegmentInfo. This class cannot be inherited.
    /// </summary>
    public sealed class SegmentInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SegmentInfo"/> class.
        /// </summary>
        /// <param name="name">The name of the segment.</param>
        /// <param name="documentCount">The number of documents in the segment.</param>
        /// <param name="directory">The directory where the segment is stored.</param>
        public SegmentInfo(string name, int documentCount, IFileProvider directory)
        {
            this.Name = name;
            this.DocumentCount = documentCount;
            this.Directory = directory;
        }

        /// <summary>
        /// Gets the name of the segment
        /// </summary>
        /// <value>The name.</value>
        public string Name { get; private set; }

        /// <summary>
        /// Gets the number of documents stored in the segment.
        /// </summary>
        /// <value>The document count.</value>
        public int DocumentCount { get; private set; }

        /// <summary>
        /// Gets the directory where the segment is stored.
        /// </summary>
        /// <value>The directory.</value>
        public IFileProvider Directory { get; private set; }
    }
}
