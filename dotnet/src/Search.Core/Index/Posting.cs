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
    /// Info about a <see cref="Term"/> in a <see cref="Documents.Document"/>
    /// </summary>
    internal sealed class Posting
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Posting"/> class.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <param name="position">The position.</param>
        public Posting(Term term, int position)
        {
            this.Term = term;
            this.Frequency = 1;
            this.Positions = new int[1];
            this.Positions[0] = position;
        }

        /// <summary>
        /// Gets or sets the frequency.
        /// </summary>
        /// <value>The frequency.</value>
        public int Frequency { get; set; }

        /// <summary>
        /// Gets or sets the positions.
        /// </summary>
        /// <value>The positions.</value>
        public int[] Positions { get; set; }

        /// <summary>
        /// Gets the term.
        /// </summary>
        /// <value>The term.</value>
        public Term Term { get; private set; }
    }
}
