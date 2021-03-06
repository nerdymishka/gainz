﻿/**
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
using System.Collections.Generic;
using System.IO;
using NerdyMishka.Search.Collections;
using NerdyMishka.Search.IO;

namespace NerdyMishka.Search.Index
{
    /// <summary>
    /// Manages the segments file. 
    /// </summary>
    public class SegmentInfoList : ThreadSafeList<SegmentInfo>
    {
        /// <summary>
        /// Gets or sets the counter.
        /// </summary>
        /// <value>The counter.</value>
        internal int Counter { get; set; }

        /// <summary>
        /// Reads the file of segments into the current instance of <see cref="SegmentInfo"/>
        /// </summary>
        /// <param name="directory">The directory to read from.</param>
        public void Read(IFileProvider directory)
        {
            lock(this.syncRoot)
            {
                Check.NullParamenter(nameof(directory), directory);
                using (var reader = directory.OpenReader("segments")) 
                {
                    this.Counter = reader.ReadInt32();
                    int i = reader.ReadInt32();
                    for (; i > 0; i--)
                    {
                        var si = new SegmentInfo(reader.ReadString(), reader.ReadInt32(), directory);
                        this.list.Add(si);
                    }
                }
            }
        }
      
        /// <summary>
        /// Writes the segment file to directory.
        /// </summary>
        /// <param name="directory">The directory that will be written to.</param>
        public void Write(IFileProvider directory)
        {
            Check.NullParamenter(nameof(directory), directory);
            lock(this.syncRoot)
            {
                using (var writer = directory.OpenWriter("segments.new"))
                {
                    writer.Write(this.Counter);
                    writer.Write(this.Count);
                    for (int i = 0; i < this.Count; i++)
                    {
                        var si = this.list[i];
                        writer.Write(si.Name);
                        writer.Write(si.DocumentCount);
                    }
                }

                // move to segments file.
                directory.Move("segments.new", "segments", true);
            }
          
        }
    }
}
