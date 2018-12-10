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
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.Search.Index
{
    /// <summary>
    /// Interface ITermPositionsEnumerator is a contract for enumerators that provide
    /// the <see cref="DocumentFrequencyPair"/> and then allows the consumer to enumerate
    /// over the positions of each occurrence of a term within the document. This is 
    /// the <c>TermPositions</c> in Java.
    /// </summary>
    /// <seealso cref="BadMishka.DocumentFormat.LuceneIndex.Index.IDocumentTermEnumerator" />
    public interface ITermPositionEnumerator : ITermEnumerator
    {
        /// <summary>
        /// Reads next position in the current document.  It is an error to call
        /// this more than <see cref="ITermDocs.Frequency"/> times
        /// without calling <see cref="ITermDocs.Next"/> This is
        /// invalid until <see cref="ITermDocs.Next"/> is called for
        /// the first time.
        /// </summary>
        /// <returns>The next position; otherwise -1</returns>
        int ReadNextPosition();
    }
}
