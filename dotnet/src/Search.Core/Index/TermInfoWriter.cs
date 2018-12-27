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
using NerdyMishka.Search.IO;

namespace NerdyMishka.Search.Index
{
    /// <summary>
    /// This stores <see cref="Term"/> and <see cref="TermInfo"/> pairs in a <see cref="IDirectory"/>.
    /// Pairs are read either by <see cref="Term"/> or by the ordinal position.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public class TermInfoWriter : IDisposable
    {
        /// <summary>
        /// The index interval
        /// </summary>
        public const int IndexInterval = 128;

        /// <summary>
        /// The field information list
        /// </summary>
        private FieldInfoList fieldInfoList;

        /// <summary>
        /// The writer
        /// </summary>
        private IBinaryWriter writer;

        /// <summary>
        /// The last term
        /// </summary>
        private Term lastTerm = new Term(string.Empty, string.Empty);

        /// <summary>
        /// The last term information
        /// </summary>
        private TermInfo lastTermInfo = new TermInfo();

        /// <summary>
        /// The size/
        /// </summary>
        private long count = 0;

        /// <summary>
        /// The last index pointer
        /// </summary>
        private long lastIndexPointer = 0;

        /// <summary>
        /// The is index/
        /// </summary>
        private bool isIndex = false;

        /// <summary>
        /// The other
        /// </summary>
        private TermInfoWriter other = null;

        /// <summary>
        /// Initializes a new instance of the <see cref="TermInfoListWriter"/> class.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="segmentName">Name of the segment.</param>
        /// <param name="fieldInfoList">The field info list.</param>
        public TermInfoWriter(IFileProvider directory, string segmentName, FieldInfoList fieldInfoList)
        {
            this.Initialize(directory, segmentName, fieldInfoList, false);
            this.other = new TermInfoWriter(directory, segmentName, fieldInfoList, true);
            this.other.other = this;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TermInfoListWriter"/> class.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="segmentName">The segment.</param>
        /// <param name="fieldInfoList">The field info list.</param>
        /// <param name="isIndex">if set to <c>true</c> [is index].</param>
        private TermInfoWriter(IFileProvider directory, string segmentName, FieldInfoList fieldInfoList, bool isIndex)
        {
            this.Initialize(directory, segmentName, fieldInfoList, isIndex);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.writer.Seek(4); // write size after format
            this.writer.Write(this.count);
            this.writer.Dispose();
            this.writer = null;

            if (!this.isIndex)
            {
                if (this.other != null)
                    this.other.Dispose();
            }
        }

        /// <summary>
        /// Adds the term and termInfo to the output.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <param name="termInfo">The term information.</param>
        /// <exception cref="System.IO.IOException">
        /// term out of order
        /// or
        /// frequencyPointer out of order
        /// or
        /// proxyPointer out of order
        /// </exception>
        public void Write(Term term, TermInfo termInfo)
        {
            if (!this.isIndex && term.CompareTo(this.lastTerm) <= 0)
                throw new System.IO.IOException("term out of order");

            if (termInfo.FrequencyPointer < this.lastTermInfo.FrequencyPointer)
                throw new System.IO.IOException("frequencyPointer out of order");

            if (termInfo.ProxyPointer < this.lastTermInfo.ProxyPointer)
                throw new System.IO.IOException("proxyPointer out of order");

            if (!this.isIndex && this.count % IndexInterval == 0)
                this.other.Write(this.lastTerm, this.lastTermInfo); // add an index term

            this.WriteTerm(term); // write term
            this.writer.WriteVariableLengthInt(termInfo.DocumentFrequency); // write doc freq
            this.writer.WriteVariableLengthInt(termInfo.FrequencyPointer - this.lastTermInfo.FrequencyPointer); // write pointers
            this.writer.WriteVariableLengthInt(termInfo.ProxyPointer - this.lastTermInfo.ProxyPointer);

            if (this.isIndex)
            {
                this.writer.WriteVariableLengthInt(this.other.writer.Position - this.lastIndexPointer);
                this.lastIndexPointer = this.other.writer.Position; // write pointer
            }

            this.lastTermInfo.Set(termInfo);
            this.count++;
        }

        /// <summary>
        /// Initializes the specified directory.
        /// </summary>
        /// <param name="directory">The directory.</param>
        /// <param name="segmentName">Name of the segment.</param>
        /// <param name="fieldInfoList">The field information list.</param>
        /// <param name="isIndex">if set to <c>true</c> [is index].</param>
        private void Initialize(IFileProvider directory, string segmentName, FieldInfoList fieldInfoList, bool isIndex)
        {
            this.fieldInfoList = fieldInfoList;
            this.isIndex = isIndex;
            this.writer = directory.OpenWriter(segmentName + (this.isIndex ? ".tii" : ".tis"));
            this.writer.Write(0); // leave space for size
        }

        /// <summary>
        /// Writes the term to the write stream.
        /// </summary>
        /// <param name="term">The term.</param>
        private void WriteTerm(Term term)
        {
            int start = this.lastTerm.Text.DivergentIndex(term.Text);
            int length = term.Text.Length - start;

            this.writer.WriteVariableLengthInt(start); // write shared prefix length
            this.writer.WriteVariableLengthInt(length); // write delta length
            this.writer.Write(term.Text.ToCharArray(), start, length); // write delta chars

            this.writer.WriteVariableLengthInt(this.fieldInfoList.IndexOf(term.FieldName)); // write Field num

            this.lastTerm = term;
        }        
    }
}
