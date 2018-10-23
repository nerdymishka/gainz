using NerdyMishka.Data;

namespace NerdyMishka.Data
{
    public class ReferenceConstraint : DbConstraint
    {
        

        public DbColumn Owner { get; internal protected set; }

        public DbColumn ForeignColumn { get; internal protected set; }

        public string ForeignColumnName { get => this.ForeignColumn.Name; }

        public string ForeignTableName { get => this.ForeignColumn.Table.Name; }

        public ReferenceAction Update { get; set ;}

        public ReferenceAction Delete { get; set; }
    }

    public enum ReferenceAction
    {
        NoAction = 0,
        Cascade = 1,
        SetNull = 2,
        SetDefault = 3
    }
}