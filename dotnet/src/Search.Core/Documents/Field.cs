
using System;

namespace NerdyMishka.Search.Documents
{
    public enum StorageStrategy
    {
        None,

        Store
    }

    public enum IndexStrategy
    {
        None,

        Analyzed,

        NotAnalyzed 
    }

    public class Field : IField
    {

        public Field(
            string name, 
            string value, 
            StorageStrategy storage = StorageStrategy.None, 
            IndexStrategy index = IndexStrategy.None) 
        {
            this.Name = name;
            this.Value = value;
            this.Storage = storage;
            this.Index = index;
        }

        public Field(
            string name, 
            System.IO.TextReader reader, 
            StorageStrategy storage = StorageStrategy.None, 
            IndexStrategy index = IndexStrategy.None) 
        {
            this.Name = name;
            this.Reader = reader;
            this.Index = index;
            this.Storage = storage;
        }

        public Field(
            string name, 
            DateTime dateTime, 
            StorageStrategy storage = StorageStrategy.None, 
            IndexStrategy index = IndexStrategy.None) 
        {
            this.Value = Epoch.FromDateTime(dateTime)
                .ToString();

            this.Name = name;
            this.Storage = storage;
            this.Index = index;
        }

        

        public virtual string Name { get; protected set;}

        public virtual StorageStrategy Storage { get; protected set; }

        public virtual IndexStrategy Index { get; protected set; }

        public virtual string Value { get; set; }

        public virtual System.IO.TextReader Reader { get; set; }

        public static Field Keyword(string name, string value) 
        {
            return new Field(
                name, 
                value, 
                StorageStrategy.Store, 
                IndexStrategy.NotAnalyzed);
        }

        /// <summary>
        /// Creates a stored field that isn't indexed. Similar to 
        /// Unindexed in lucene.0
        /// </summary>
        /// <remarks>
        /// - value: yes
        /// - reader: no
        /// - stored: yes
        /// - indexed: no
        /// - tokenized: no
        /// </remarks>
        /// <param name="name"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Field Stored(string name, string value) 
        {
            return new Field(
                name, 
                value, 
                StorageStrategy.Store);
        }

        public static Field Stored(string name, DateTime value)
        {
            return new Field(name, value, StorageStrategy.Store);
        }

        public static Field Indexed(string name, string value)
        {
            return new Field(name, value, index: IndexStrategy.Analyzed);   
        }

        public static Field Indexed(string name, DateTime value)
        {
            return new Field(name, value, index: IndexStrategy.Analyzed);
        }

        public static Field Text(string name, string value) 
        {
            return new Field(
                name, 
                value,
                StorageStrategy.Store,
                IndexStrategy.Analyzed);
        }

        public static Field Text(string name, DateTime value)
        {
            return new Field(
                name, 
                value, 
                StorageStrategy.Store,
                IndexStrategy.Analyzed
            );
        }

        public static Field Text(string name, System.IO.TextReader reader)
        {
            return new Field(name, reader);
        }
 
    }
}