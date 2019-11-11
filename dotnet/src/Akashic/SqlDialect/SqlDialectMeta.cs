


namespace NerdyMishka.Data
{
    public abstract class SqlDialectMeta
    {

        public virtual void TableColumns(IDataConnection connection, string tableName)
        {

        }
       
    }

    public class ColumnInfo 
    {
        public string TableCatalog { get; set; }

        public string TableSchema { get; set; }


        public string TableName { get; set; }

        public string ColumnName { get; set; }

        public int Ordinal { get; set; }

        public bool IsNullable { get; set; }
        
        public string DefaultValue { get; set; }
        public string SqlType { get; set; }

        public int? Size { get; set; }
        
        public int? Precision  {get; set; }

        public int? Scale  {get; set; }

        public string CharacterSetName  { get; set;  }
    }
}