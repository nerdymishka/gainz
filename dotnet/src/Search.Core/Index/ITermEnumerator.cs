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
    /// Enumerates over <see cref="DocumentFrequencyPair"/>. 
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface ITermEnumerator : IEnumerator<DocumentFrequencyPair>, IDisposable
    {
        /// <summary>
        /// Reads the number of documents specified by the length of the array
        /// if the enumerator contains the amount given.
        /// </summary>
        /// <param name="documentIndexes">The document indexes.</param>
        /// <param name="frequencies">The frequencies.</param>
        /// <returns>The number of documentIndexes that were read.</returns>
        int Read(int[] documentIndexes, int[] frequencies);

        /// <summary>
        /// Skips entries to the first beyond the current whose document number is
        /// greater than or equal to *target*. Returns true if the <param name="documentIndex" />
        /// was found.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         Implementations MAY vary.  
        ///     </para>
        ///     <code lang="csharp">
        ///         private boolean SkipTo(int documentIndex) {
        ///              do {
        ///                if (!this.MoveNext())
        ///                     return false;
        ///             } while (target > this.Current.Index);
        ///             return true;
        ///         }
        ///     </code>
        /// </remarks>

        bool SkipTo(int documentIndex);
    }
}
