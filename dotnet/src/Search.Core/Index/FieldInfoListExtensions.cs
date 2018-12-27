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
    /// Extension methods for <see cref="FieldInfoList"/>
    /// </summary>
    public static class FieldInfoListExtensions
    {


        public static void AddFields(this FieldInfoList list, Document document)
        {
            foreach (var field in document)
            {
                list.Add(field.Name, field.Index  != IndexStrategy.None, false);
            }
        }

        /// <summary>
        /// Serializes the specified directory.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="fileName">Name of the file.</param>
        public static void Serialize(this FieldInfoList collection, IFileProvider directory, string fileName)
        {
            var writer = directory.OpenWriter(fileName);
            try
            {
                Serialize(collection, writer);
            }
            finally
            {
                writer.Dispose();
            }
        }

        /// <summary>
        /// Deserializes <see cref="FieldInfoList"/> from a given <see cref="IDirectory"/>.
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="directory">The directory.</param>
        /// <param name="fileName">Name of the file.</param>
        public static void Deserialize(this FieldInfoList collection, IFileProvider directory, string fileName)
        {
            var reader = directory.OpenReader(fileName);
            try
            {
                Deserialize(collection, reader);
            }
            finally
            {
                reader.Dispose();
            }
        }

        /// <summary>
        /// Serializes the <see cref="FieldInfoList"/>  
        /// </summary>
        /// <param name="list">The collection.</param>
        /// <param name="writer">The writer.</param>
        public static void Serialize(this FieldInfoList list, IBinaryWriter writer)
        {
            writer.WriteVariableLengthInt(list.Count);
            int i = 0, l = list.Count;

            for (; i < l; i++)
            {
                var field = list[i];
                byte flags = 0x0;
                if (field.IsIndexed)
                    flags |= 0x1;
                if (field.IsTermVectorStored)
                    flags |= 0x2;

                writer.Write(field.Name);
                writer.Write(flags);
            }
        }

        /// <summary>
        /// Deserializes the <see cref="FieldInfoList"/> from the <see cref="ILuceneBinaryReader"/>
        /// </summary>
        /// <param name="collection">The collection.</param>
        /// <param name="reader">The reader.</param>
        public static void Deserialize(this FieldInfoList collection, IBinaryReader reader)
        {
            int i = 0,
                l = reader.ReadVariableLengthInt32();

            for (; i < l; i++)
            {
                var name = reader.ReadString();
                byte flags = reader.ReadByte();
                bool isIndexed = (flags & 0x1) != 0,
                     storeTermVector = (flags & 0x2) != 0;

                collection.Capture(name, isIndexed, storeTermVector);
            }
        }
    }
}
