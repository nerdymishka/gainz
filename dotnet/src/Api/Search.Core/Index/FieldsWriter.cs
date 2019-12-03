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
using NerdyMishka.Search.Documents;
using NerdyMishka.Search.IO;

namespace NerdyMishka.Search.Index
{
    /// <summary>
    /// Class FieldsWriter.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class FieldsWriter : IDisposable
    {
        /// <summary>
        /// The field information collection
        /// </summary>
        private FieldInfoList fieldInfoList;

        /// <summary>
        /// The fields stream
        /// </summary>
        private IBinaryWriter fieldsStream;

        /// <summary>
        /// The index stream
        /// </summary>
        private IBinaryWriter indexStream;

        /// <summary>
        /// Initializes a new instance of the <see cref="FieldsWriter"/> class.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="segment">The segment.</param>
        /// <param name="fieldInfoList">The field information collection.</param>
        internal FieldsWriter(IFileProvider directory, string segment, FieldInfoList fieldInfoList)
        {
            this.fieldInfoList = fieldInfoList;
            this.fieldsStream = directory.OpenWriter(segment + ".fdt");
            this.indexStream = directory.OpenWriter(segment + ".fdx");
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            if (this.fieldsStream != null)
                this.fieldsStream.Dispose();

            if (this.indexStream != null)
                this.indexStream.Dispose();
        }

        /// <summary>
        /// Adds the document.
        /// </summary>
        /// <param name="doc">The document.</param>
        internal void AddDocument(Document doc)
        {
            int storedCount = 0;

            this.indexStream.Write(this.fieldsStream.Position);

            foreach (Field field in doc)
            {
                if (field.Storage == StorageStrategy.Store)
                    storedCount++;
            }

            this.fieldsStream.WriteVariableLengthInt(storedCount);

            foreach (Field field in doc)
            {
                if (field.Storage == StorageStrategy.Store)
                {
                    this.fieldsStream.WriteVariableLengthInt(this.fieldInfoList.IndexOf(field.Name));

                    byte bits = 0;
                    if (field.Index == IndexStrategy.Analyzed)
                        bits |= 1;

                    this.fieldsStream.Write(bits);
                    this.fieldsStream.Write(field.Value);
                }
            }
        }
    }
}
