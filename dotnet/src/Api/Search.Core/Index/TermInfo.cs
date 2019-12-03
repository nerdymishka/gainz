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
namespace NerdyMishka.Search.Index
{
    /// <summary>
    /// The record of information for a <see cref="Term"/>. This class cannot be inherited. 
    /// </summary>
    public sealed class TermInfo : ICloneable<TermInfo>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TermInfo"/> class.
        /// </summary>
        public TermInfo()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TermInfo"/> class.
        /// </summary>
        /// <param name="documentFrequency">The frequency a term appears in the document.</param>
        /// <param name="frequencyPointer">The frequency pointer.</param>
        /// <param name="proxyPointer">The proxy pointer.</param>
        public TermInfo(int documentFrequency, long frequencyPointer, long proxyPointer)
        {
            this.Set(documentFrequency, frequencyPointer, proxyPointer);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TermInfo"/> class.
        /// </summary>
        /// <param name="termInfo">The term information.</param>
        internal TermInfo(TermInfo termInfo)
        {
            this.Set(termInfo);
        }

        /// <summary>
        /// Gets the document frequency.
        /// </summary>
        /// <value>The document frequency.</value>
        public int DocumentFrequency { get; private set; }

        /// <summary>
        /// Gets the frequency pointer.
        /// </summary>
        /// <value>The frequency pointer.</value>
        public long FrequencyPointer { get; private set; }

        /// <summary>
        /// Gets the proxy pointer.
        /// </summary>
        /// <value>The proxy pointer.</value>
        public long ProxyPointer { get; private set; }

        /// <summary>
         /// Clones the instance of the current object.
        /// </summary>
        /// <param name="deep">If true, a deep clone is requested.</param>
        /// <returns>A clone of the instance of the current object.</returns>
        public TermInfo Clone()
        {
            return new TermInfo(this.DocumentFrequency, this.FrequencyPointer, this.ProxyPointer);
        }

        /// <summary>
        /// Sets the specified term information.
        /// </summary>
        /// <param name="termInfo">The term information.</param>
        internal void Set(TermInfo termInfo)
        {
            this.DocumentFrequency = termInfo.DocumentFrequency;
            this.FrequencyPointer = termInfo.FrequencyPointer;
            this.ProxyPointer = termInfo.ProxyPointer;
        }

        /// <summary>
        /// Sets the <see cref="TermInfo"/> values.
        /// </summary>
        /// <param name="documentFrequency">The document frequency.</param>
        /// <param name="frequencyPointer">The frequency pointer.</param>
        /// <param name="proxyPointer">The proxy pointer.</param>
        internal void Set(int documentFrequency, long frequencyPointer, long proxyPointer)
        {
            this.DocumentFrequency = documentFrequency;
            this.FrequencyPointer = frequencyPointer;
            this.ProxyPointer = proxyPointer;
        }
    }
}
