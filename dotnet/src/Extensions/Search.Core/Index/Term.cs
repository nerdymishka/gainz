/*
Copyright 2016 Nerdy Mishka LLC

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

namespace NerdyMishka.Search.Index
{
    /// <summary>
    ///  A Term represents a word from text. 
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This is the unit of search.  It is composed of two elements, 
    ///         the text of the word, as a string, and the name of the field that
    ///         the text occurred in, an interned string.
    ///     </para>
    ///     <para>
    ///         Terms MAY represent more than words from text fields such as
    ///         dates, e-mail addresses, URLs, etc. 
    ///     </para>
    /// </remarks>
    public class Term : IComparable, IComparable<Term>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Term" /> class.
        /// </summary>
        /// <param name="fieldName">The name of the field the term is associated with.</param>
        /// <param name="text">The string representation of the stored data.</param>
        /// <param name="intern">Forces the term to intern the <see cref="FieldName" /></param>
        public Term(string fieldName, string text, bool intern = false)
        {
            this.Set(string.Intern(fieldName), text);
        }

        /// <summary>
        /// Gets the name of the field. The field is data column within
        /// a document.
        /// </summary>
        /// <value>The name of the field.</value>
        public string FieldName { get; private set; }

        /// <summary>
        /// Gets the text of the <see cref="Term"/>. The term MAY be a word
        /// or the textual representation of data such as a date, integer, e-mail address
        /// url. etc 
        /// </summary>
        /// <value>The text.</value>
        public string Text { get; private set; }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" /> is equal to this instance.
        /// </summary>
        /// <param name="obj">The object to compare with the current object.</param>
        /// <returns><c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.</returns>
        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is Term)
            {
                var right = (Term)obj;
                return this.FieldName == right.FieldName && this.Text == right.Text;
            }

            return false;
        }

        /// <summary>
        /// Compares the current instance with another object of the same type and returns an integer that indicates whether the current instance precedes, follows, or occurs in the same position in the sort order as the other object.
        /// </summary>
        /// <param name="obj">An object to compare with this instance.</param>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has these meanings: Value Meaning Less than zero This instance precedes <paramref name="obj" /> in the sort order. Zero This instance occurs in the same position in the sort order as <paramref name="obj" />. Greater than zero This instance follows <paramref name="obj" /> in the sort order.</returns>
        public int CompareTo(object obj)
        {
            return this.CompareTo((Term)obj);
        }

        /// <summary>
        /// Compares the current object with another object of the same type.
        /// </summary>
        /// <param name="other">An object to compare with this object.</param>
        /// <returns>A value that indicates the relative order of the objects being compared. The return value has the following meanings: Value Meaning Less than zero This object is less than the <paramref name="other" /> parameter.Zero This object is equal to <paramref name="other" />. Greater than zero This object is greater than <paramref name="other" />.</returns>
        public int CompareTo(Term other)
        {
            if (this.FieldName == other.FieldName)
                return string.CompareOrdinal(this.Text, other.Text);

            return string.CompareOrdinal(this.FieldName, other.FieldName);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.</returns>
        public override int GetHashCode()
        {
            return this.FieldName.GetHashCode() + this.Text.GetHashCode();
        }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>A <see cref="System.String" /> that represents this instance.</returns>
        public override string ToString()
        {
            return this.FieldName + ":" + this.Text;
        }

        /// <summary>
        /// Sets the values of the specified <see cref="Term"/>.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="text">The text.</param>
        internal void Set(string field, string text)
        {
            this.FieldName = field;
            this.Text = text;
        }
    }
}
