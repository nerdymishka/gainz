using System;
using System.Collections.Generic;
using System.Data;

namespace NerdyMishka.Data
{
    public interface IColumn : IColumnReference
    {
        bool IsNullable { get; set; }

        int? Precision { get; set; }

        int? Scale { get; set; }

        int? Limit { get; set; }

        Type ClrType { get; set; }

        AkashicDbType DbType { get; set; }
        string DefaultValue { get; set; }
       
        void Add(IConstraintReference constraint);

        void Remove(IConstraintReference constraint);
    }
}