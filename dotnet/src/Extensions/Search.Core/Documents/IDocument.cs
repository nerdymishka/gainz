
using System.Collections.Generic;

namespace NerdyMishka.Search.Documents
{
    public interface IDocument : IEnumerable<IField>
    {
        void Add(IField field);

        bool Remove(string fieldName, bool all = false);

        IField this[string fieldName] { get; }

        string GetValue(string fieldName);

        string[] GetValues(string fieldName);

        IField[] GetFields(string fieldName);

        bool Contains(string fieldName);
    }
}