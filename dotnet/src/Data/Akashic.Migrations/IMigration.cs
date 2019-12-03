using System;
using System.ComponentModel.Composition;
using NerdyMishka.Data;

namespace NerdyMishka.Data.Migrations
{
    [InheritedExport]
	public interface IMigration: IMigrationVersion
	{
		void Up();
		
        void Down();
        ISqlExecutor SqlExecutor { get; set; }	
	}
}
