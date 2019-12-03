using System;
using System.ComponentModel.Composition;
using NerdyMishka.Data;


namespace NerdyMishka.Data.Migrations
{
    [InheritedExport]
    public interface IDatabaseInitializer
    {
        IDataConnection Connection { get; set; }

        AkashicFactory Factory { get; set; }

        void Create(bool drop = false);
    }
}