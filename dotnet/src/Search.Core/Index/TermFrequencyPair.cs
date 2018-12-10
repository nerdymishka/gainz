using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.Search.Index
{
    /// <summary>
    /// Class TermFrequencyPair.
    /// </summary>
    public class TermFrequencyPair
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TermFrequencyPair"/> class.
        /// </summary>
        /// <param name="term">The term.</param>
        /// <param name="frequency">The frequency.</param>
        public TermFrequencyPair(Term term, int frequency)
        {
            this.Term = term;
            this.Frequency = frequency;
        }

        /// <summary>
        /// Gets or sets the term.
        /// </summary>
        /// <value>The term.</value>
        public Term Term { get; set; }

        /// <summary>
        /// Gets or sets the frequency.
        /// </summary>
        /// <value>The frequency.</value>
        public int Frequency { get; set; }
    }
}
