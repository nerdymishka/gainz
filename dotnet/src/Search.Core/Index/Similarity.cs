/**
 * Copyright 2016 Bad Mishka LLC
 * Based Upon Lucene from The Apache Foundation, Copyright 2004
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.Search.Index
{
    public abstract class Similarity
    {
        private static readonly float[] s_normTable;

        static Similarity()
        {
            s_normTable = CreateNormalizedTable();
        }

        static float[] CreateNormalizedTable()
        {
            float[] result = new float[256];
            for (int i = 0; i < 256; i++)
                result[i] = i / 255.0F;
            return result;
        }

        /// <summary>
        /// Normalizes the value by un-scaling the byte value into a float 
        /// e.g. approximately 1/sqrt(numTerms)
        /// </summary>
        /// <param name="value">The value to normalize.</param>
        /// <returns>The normalized value.</returns>
        internal static float Normalize(byte value)
        {
            return s_normTable[value & 0xFF];
        }

        /// <summary>
        /// Calculates the coordination factor, a value that rewards documents with a higher
        /// score when matching more terms of a query.
        /// </summary>
        /// <param name="overlap">The number of overlaps found in a document.</param>
        /// <param name="maxOverlap">The max number of overlaps allowed per document.</param>
        /// <returns>The coordination factor.</returns>
        internal static float CalculateCoordinationFactor(int overlap, int maxOverlap)
        {
            return overlap / (float)maxOverlap;
        }

        /// <summary>
        /// Returns the calculated term frequency for a document. e.g. tf(terms in document) = sqrt(frequency)
        /// </summary>
        /// <param name="frequencyOfTerm">The number of times a term appears in a document.</param>
        /// <returns>The term frequency.</returns>
        public static float CalculateTermFrequency(int frequencyOfTerm)
        {
            return (float)Math.Sqrt(frequencyOfTerm);
        }

        /// <summary>
        /// Returns the calculated term frequency for a document. e.g. tf(terms in document) = sqrt(frequency)
        /// </summary>
        /// <param name="frequencyOfTerm">The number of times a term appears in a document.</param>
        /// <returns>The term frequency.</returns>
        public static float CalculateTermFrequency(float frequencyOfTerm)
        {
            return (float)Math.Sqrt(frequencyOfTerm);
        }

        public static float CalculateInverseDocumentFrequency(int documentFrequncy, int totalDocumentCount)
        {
            return (float)(Math.Log(totalDocumentCount / (double)(documentFrequncy + 1)) + 1.0);
        }

        /// <summary>
        /// Encodes a normalization factor for storage in an index.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         The encoding uses a five-bit exponent and three-bit mantissa, thus
        ///         representing values from around 7x10^9 to 2x10^-9 with about one
        ///         significant decimal digit of accuracy.  Zero is also represented.
        ///         Negative numbers are rounded up to zero.  
        ///     </para>
        ///     <para>
        ///         Values too large to represent
        ///         are rounded down to the largest representable value.  Positive values too
        ///         small to represent are rounded up to the smallest positive representable
        ///         value.
        ///     </para>
        /// </remarks>
        /// <param name="value">The value to be encoded.</param>
        /// <returns>The encoded <see cref="byte"/> value of the float.</returns>
        /// <see cref="Document.Field.Boost"/>
        public static byte EncodeNormalization(float value)
        {
            return value.ToByte();
        }
       

        /// <summary>
        /// Computes the normalization value for a Field given the total number of
		/// terms contained in a Field.
        /// </summary>
        /// <remarks>
        ///     <para>
        ///         These values, together with Field boosts, are
        ///         stored in an index and multipled into scores for hits on each Field by the
        ///         search code.   
        ///     </para>
        ///     <para>
        ///         Matches in longer fields are less precise, so implemenations of this
        ///         method usually return smaller values when <paramref name="tokenCount"/> is large,
        ///         and larger values when <paramref name="tokenCount"/> is small.
        ///     </para>
        /// </remarks>
        /// <param name="fieldName">The name of the <see cref="Document.Field"/></param>
        /// <param name="tokenCount">
        ///     The total number of tokens contained in all <see cref="Document.Field"/>s with 
        ///     all the <paramref name="fieldName"/> in the <see cref="Document.Document"/> 
        /// </param>
        /// <returns>
        /// The <see cref="float"/> normalization factor for hits on this <see cref="Document.Field"/> of the
        /// <see cref="Document.Document"/>.
        /// </returns>
        public abstract float LengthNormalization(string fieldName, int tokenCount);

       
    }
}
