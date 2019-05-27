

using System;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NerdyMishka.EfCore.Metadata;

public class NerdyMishkaOptionsExtension : IDbContextOptionsExtension
{
    private string logFragment;
    private long code = 0;

    public NerdyMishkaOptionsExtension()
    {

    }

    protected NerdyMishkaOptionsExtension(NerdyMishkaOptionsExtension from)
    {
        this.DefaultSchemaName = from.DefaultSchemaName;
        this.MigrationSchemaName = from.MigrationSchemaName;
        this.MigrationTableName = from.MigrationTableName;
        this.Conventions = from.Conventions;
    }

    public string DefaultSchemaName { get; protected set; }

    public string TablePrefix { get; protected set; }

    public string MigrationSchemaName { get; protected set; } 

    public string MigrationTableName { get; protected set; } = "EfMigrationHistory";

    public IConstraintConventions Conventions { get; protected set; }

    protected virtual NerdyMishkaOptionsExtension Clone()
    {
        return new NerdyMishkaOptionsExtension(this);
    }

    public NerdyMishkaOptionsExtension WithConventions(IConstraintConventions conventions)
    {
        var clone = this.Clone();
        clone.Conventions = conventions;
        return clone;
    }

    public NerdyMishkaOptionsExtension WithTablePrefix(string tablePrefix)
    {
        var clone = this.Clone();
        clone.TablePrefix = tablePrefix;
        return clone;
    }


    public NerdyMishkaOptionsExtension WithMigrationTableName(string tableName)
    {
        var clone = this.Clone();
        clone.MigrationTableName = tableName;
      

        return clone;
    }

    public NerdyMishkaOptionsExtension WithMigrationSchemaName(string schemaName)
    {
        var clone = this.Clone();
        clone.MigrationSchemaName = schemaName;
        return clone;
    }

    public NerdyMishkaOptionsExtension WithDefaultSchemaName(string schemaName)
    {
        var clone = this.Clone();
        clone.DefaultSchemaName = schemaName;
        return clone;
    }

    public string LogFragment {
        get {
            if (this.logFragment == null)
            {
                var builder = new StringBuilder();

                if(!string.IsNullOrWhiteSpace(this.TablePrefix))
                    builder.Append($"TablePrefix:{this.TablePrefix} ");

                if(!string.IsNullOrWhiteSpace(this.DefaultSchemaName))
                    builder.Append($"TablePrefix:{this.DefaultSchemaName} ");

                if(!string.IsNullOrWhiteSpace(this.MigrationSchemaName))
                    builder.Append($"MigrationSchemaName:{this.MigrationSchemaName} ");

                if(!string.IsNullOrWhiteSpace(this.MigrationTableName))
                    builder.Append($"MigrationTableName:{this.MigrationTableName} ");

                this.logFragment = builder.ToString();
            }

            return this.logFragment;
        }
    }

    

    public bool ApplyServices(IServiceCollection services)
    {
        
       
        return true;
    }

    public long GetServiceProviderHashCode()
    {
        if(this.code == 0)
        {
            var code = this.GetHashCode() * 31;
            
            if(!string.IsNullOrWhiteSpace(this.TablePrefix))
                code += this.TablePrefix.GetHashCode();

            if(!string.IsNullOrWhiteSpace(this.DefaultSchemaName))
                code += this.DefaultSchemaName.GetHashCode();

            if(!string.IsNullOrWhiteSpace(this.MigrationTableName))
                code += this.MigrationTableName.GetHashCode();

            if(!string.IsNullOrWhiteSpace(this.MigrationSchemaName))
                code += this.MigrationSchemaName.GetHashCode();
        }
        

        return code;
    }

    public void Validate(IDbContextOptions options)
    {
        
    }

    public static NerdyMishkaOptionsExtension Extract(IDbContextOptions options)
    {
        if(options == null)
            throw new ArgumentNullException(nameof(options));

        var extensions
            = options.Extensions
                .OfType<NerdyMishkaOptionsExtension>()
                .ToList();

        if (extensions.Count == 0)
        {
            throw new InvalidOperationException("No provider configured");
        }

        if (extensions.Count > 1)
        {
            throw new InvalidOperationException("Multiple providers configured");
        }

        return extensions[0];
    }
}