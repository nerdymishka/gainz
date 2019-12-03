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
using NerdyMishka.Search.Documents;
using NerdyMishka.Search.IO;

namespace NerdyMishka.Search.Index
{
    /// <summary>
    /// Class <see cref="FieldReader"/> reads the fields from a given <see cref="IDirectory"/>. This class cannot be inherited.
    /// </summary>
    public sealed class FieldReader
    {
        /// <summary>
        /// The field information collection
        /// </summary>
        private FieldInfoList fieldInfoList;

        /// <summary>
        /// The fields stream
        /// </summary>
        private IBinaryReader fieldStream;

        /// <summary>
        /// The index stream
        /// </summary>
        private IBinaryReader indexStream;

        /// <summary>
        /// The length
        /// </summary>
        private int length;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldReader"/> class.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="segmentName">The name of the segment.</param>
        /// <param name="fieldInfoList">The field information collection.</param>
        public FieldReader(IFileProvider provider, string segmentName, FieldInfoList fieldInfoList)
        {
            this.fieldInfoList = fieldInfoList;
            this.fieldStream = provider.OpenReader(segmentName + ".fdt");
            this.indexStream = provider.OpenReader(segmentName + ".fdx");
            this.length = (int)(this.indexStream.Length / 8);
        }

        /// <summary>
        /// Gets the length of data in the stream.
        /// </summary>
        /// <value>The length.</value>
        public int Length => this.length;

        /// <summary>
        /// Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            if (this.fieldStream != null)
            {
                this.fieldStream.Dispose();
            }
                

            if (this.indexStream != null)
                this.indexStream.Dispose();
        }

        /// <summary>
        /// Reads the document at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>Document.</returns>
        public Document Read(int index)
        {
            int fieldCount = 0;
            long position = 0;
            Document doc = new Document();

            this.indexStream.Seek(index * 8L);
            position = this.indexStream.ReadInt64();
            this.fieldStream.Seek(position);
            fieldCount = this.fieldStream.ReadVariableLengthInt32();

            for (int i = 0; i < fieldCount; i++)
            {
                int fieldNumber = this.fieldStream.ReadVariableLengthInt32();
                var fi = this.fieldInfoList[fieldNumber];
                byte bits = this.fieldStream.ReadByte();
                var strat = IndexStrategy.None;
                if(fi.IsIndexed || (bits & 1) != 0)
                {
                    if((bits & 1) != 0)
                        strat = IndexStrategy.Analyzed;
                    else 
                        strat = IndexStrategy.NotAnalyzed;
                }

                doc.Add(new Field(fi.Name, this.fieldStream.ReadString(), StorageStrategy.Store, strat)); // vector
            }

            return doc;
        }
    }
}
