/*
Copyright 2016 Nerdy Mishka

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
    /// The <see cref="DocumentId"/> maps to a document containing
    /// a <see cref="Term"/>.  <see cref="Frequency"/> gives
    /// the number of times the <see cref="Term"/> occurred in each document.
    /// </summary>
    public struct DocumentFrequencyPair
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DocumentFrequencyPair"/> struct.
        /// </summary>
        /// <param name="documentId">The index.</param>
        /// <param name="frequency">The frequency.</param>
        public DocumentFrequencyPair(int documentId, int frequency)
        {
            this.DocumentId = documentId;
            this.TermFrequency = frequency;
        }

        /// <summary>
        /// Gets the id of the document where a term appears.
        /// </summary>
        /// <value>The index.</value>
        public int DocumentId { get; private set; }

        /// <summary>
        /// Gets the the number of times a term appears in a document.
        /// </summary>
        /// <value>The frequency.</value>
        public int TermFrequency { get; private set; }
    }
}
