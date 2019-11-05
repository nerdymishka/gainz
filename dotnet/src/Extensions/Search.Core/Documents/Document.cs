using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace NerdyMishka.Search.Documents 
{
    /// <summary>
    /// A document is a set of fields. Each field is a name value pair stored
    /// with the document.
    /// </summary>
    /// <remarks>
    /// Documents can have more than one field with the same name. Documents should
    /// have a field that uniquely indentifies the document.
    /// 
    /// uri: java/org/apache/lucene/document/Document.java
    /// </remarks> 
    public class Document : IDocument
    {
        private List<IField> fields = new List<IField>();

        public Document() {

        }

        public int Count => fields.Count;

        /// <summary>
        /// Adds a field
        /// </summary>
        /// <param name="field">A field.</param>
        public void Add(IField field)
        {
            this.fields.Add(field);
        }

    

        /// <summary>
        /// Gets a field by name.
        /// </summary>
        /// <value>The field.</value>
        public IField this[string name]
        {
            get
            {
                foreach(var field in this.fields) {
                    if(field.Name == name)
                        return field;
                }
                return null;
            }
        }

        /// <summary>
        /// Determines if the document contains a field
        /// with the given name.
        /// </summary>
        /// <param name="name">The name of the field.</param>
        /// <returns><c>True</c>, if a field is found; otherwwise, <c>False</c></returns>
        public bool Contains(string name)
        {
            foreach(var field in this.fields)
                if(field.Name == name)
                    return true;

            return false;
        }

        /// <summary>
        /// Returns an eumerable of fields for the given name.
        /// </summary>
        /// <param name="name">The name of the field(s).</param>
        /// <returns>A enumerable of fields.</returns>
        public IField[] GetFields(string name)
        {
            var list = new List<IField>();
            foreach(var field in this.fields) {
                if(field.Name == name && field.Reader == null)
                    list.Add(field);
            }

            if(list.Count == 0)
                return Array.Empty<IField>();

            return list.ToArray();
        }

        /// <summary>
        /// Gets the string value of a given field.
        /// </summary>
        /// <param name="name">The name of the field</param>
        /// <returns>The string value.</returns>
        public string GetValue(string name)
        {
            var field = this[name];
            if(field != null)
                return field.Value;

            return null;
        }

        public string[] GetValues(string fieldName)
        {
            var fields = this.GetFields(fieldName);
            var list = new List<string>();
            foreach(var field in fields) {
                if(field.Reader == null)
                    list.Add(field.Value);
            }

            if(list.Count == 0)
                return Array.Empty<string>();

            return list.ToArray();
        }

        public bool Remove(string fieldName, bool all = false)
        {
            if(!all)
            {
                var field = this[fieldName];
                if(field == null)
                    return false;
     
                return this.fields.Remove(field);
            }

            var fields = this.GetFields(fieldName);
            if(fields.Length == 0)
                return false;

            bool removed = false;
            foreach(var field in fields) {
                IField next = field;
                if(this.fields.Remove(next)) {
                    removed = true;
                }
            }

            return removed;
        }

        /// <summary>
        /// Returns the document in a human readable format e.g.null
        /// Document&gt;Field1 Field2&lt;
        /// </summary>
        /// <returns>A string representation of the document</returns>
        public override string ToString()
        {
            var sb = new System.Text.StringBuilder();
            sb.Append("Document<");
            var i = 0;
            foreach(var field in this.fields) {
                if(i != 0)
                    sb.Append(" ");
                sb.Append(field.ToString());
                i++;
            }
            sb.Append(">");

            return sb.ToString();
         }

        public IEnumerator<IField> GetEnumerator()
        {
            return this.fields.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }
}

