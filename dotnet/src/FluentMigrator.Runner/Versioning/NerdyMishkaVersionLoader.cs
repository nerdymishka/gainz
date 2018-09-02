using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

using FluentMigrator.Expressions;
using FluentMigrator.Model;
using FluentMigrator.Runner.Versioning;
using FluentMigrator.Runner.VersionTableInfo;
using FluentMigrator.Infrastructure;
using FluentMigrator.Runner.Conventions;
using FluentMigrator.Runner.Initialization;
using FluentMigrator.Runner.Processors;
using FluentMigrator.Runner;
using FluentMigrator;

namespace NerdyMishka.FluentMigrator.Runner.Versioning
{
   public class NerdyMishkaVersionLoader : IVersionLoader
    {

        private readonly IMigrationProcessor processor;

        private readonly IConventionSet conventionSet;
        private bool versionSchemaMigrationAlreadyRun;
        private bool versionMigrationAlreadyRun;
        private bool versionUniqueMigrationAlreadyRun;
        private bool versionDescriptionMigrationAlreadyRun;

        private bool versionModuleMigrationAlreadyRun;

        private INerdyMishkaVersionInfo versionInfo;
        private IMigrationRunnerConventions Conventions { get; set; }

        public IVersionTableMetaData VersionTableMetaData => this.NerdyMishkaVersionTableMetaData;

        public INerdyMishkaVersionTableMetaData NerdyMishkaVersionTableMetaData { get; private set;}
      
        public IMigrationRunner Runner { get; set; }
        public VersionSchemaMigration VersionSchemaMigration { get; private set; }
        public IMigration VersionMigration { get; private set;}
        public IMigration VersionUniqueMigration { get; private set; }
        public IMigration VersionDescriptionMigration { get; private set; }
        public IMigration VersionModuleMigration { get; private set;}      

        public NerdyMishkaVersionLoader(
             IProcessorAccessor processorAccessor,
             IConventionSet conventionSet,
             IMigrationRunnerConventions conventions,
             INerdyMishkaVersionTableMetaData versionTableMetaData,
             IMigrationRunner runner)
        {
            this.conventionSet = conventionSet;
            this.processor = processorAccessor.Processor;

            this.Runner = runner;

            this.Conventions = conventions;
            this.NerdyMishkaVersionTableMetaData = versionTableMetaData;
            this.VersionMigration = new VersionMigration(NerdyMishkaVersionTableMetaData);
            this.VersionSchemaMigration = new VersionSchemaMigration(NerdyMishkaVersionTableMetaData);
            this.VersionUniqueMigration = new VersionUniqueMigration(NerdyMishkaVersionTableMetaData);
            this.VersionDescriptionMigration = new VersionDescriptionMigration(VersionTableMetaData);
            this.VersionModuleMigration = new NerdyMishkaVersionModuleMigration(NerdyMishkaVersionTableMetaData);

            this.LoadVersionInfo();
        }

        public void UpdateVersionInfo(long version)
        {
            UpdateVersionInfo(version, null);
        }

        public void UpdateVersionInfo(long version, string description)
        {
            this.UpdateVersionInfo(version, description);
        }

        public void UpdateVersionInfo(long version, string description, string module)
        {
            if(string.IsNullOrWhiteSpace(module))
                module = "app";

            var dataExpression = new InsertDataExpression();
            dataExpression.Rows.Add(CreateVersionInfoInsertionData(version, description, module));
            dataExpression.TableName = VersionTableMetaData.TableName;
            dataExpression.SchemaName = VersionTableMetaData.SchemaName;

            dataExpression.ExecuteWith(processor);
        }

        
        public IVersionTableMetaData GetVersionTableMetaData()
        {
            return VersionTableMetaData;
        }

        protected virtual InsertionDataDefinition CreateVersionInfoInsertionData(long version, string description, string module)
        {
            return new InsertionDataDefinition
                       {
                           new KeyValuePair<string, object>(VersionTableMetaData.ColumnName, version),
                           new KeyValuePair<string, object>(VersionTableMetaData.AppliedOnColumnName, DateTime.UtcNow),
                           new KeyValuePair<string, object>(VersionTableMetaData.DescriptionColumnName, description),
                           new KeyValuePair<string, object>(this.NerdyMishkaVersionTableMetaData.ModuleColumnName, module)
                       };
        }

  

        public INerdyMishkaVersionInfo NerdyMishkaVersionInfo
        {
            get => this.versionInfo;
            set => this.versionInfo = value ?? 
                throw new ArgumentNullException(nameof(value));
        }

        public IVersionInfo VersionInfo
        {
            get => this.NerdyMishkaVersionInfo;
            set  {
                if(value is INerdyMishkaVersionInfo) {
                    this.NerdyMishkaVersionInfo = (INerdyMishkaVersionInfo)value;
                } else {
                    throw new InvalidCastException("VersionInfo must implement INerdyMishkaVersionInfo");
                }
            }
        }

        public bool AlreadyCreatedVersionSchema => 
            string.IsNullOrEmpty(this.VersionTableMetaData.SchemaName) ||
                processor.SchemaExists(this.VersionTableMetaData.SchemaName);

        public bool AlreadyCreatedVersionTable => 
            processor.TableExists(
                this.VersionTableMetaData.SchemaName, 
                this.VersionTableMetaData.TableName);

        public bool AlreadyMadeVersionUnique => 
            processor.ColumnExists(
                this.VersionTableMetaData.SchemaName, 
                this.VersionTableMetaData.TableName,
                this.VersionTableMetaData.AppliedOnColumnName);

        public bool AlreadyMadeVersionDescription => 
            processor.ColumnExists(
                this.VersionTableMetaData.SchemaName, 
                this.VersionTableMetaData.TableName, 
                this.VersionTableMetaData.DescriptionColumnName);

        public bool AlreadyMadeModuleDescription => 
            processor.ColumnExists(
                this.NerdyMishkaVersionTableMetaData.SchemaName,
                this.NerdyMishkaVersionTableMetaData.TableName,
                this.NerdyMishkaVersionTableMetaData.ModuleColumnName);
        public bool OwnsVersionSchema => VersionTableMetaData.OwnsSchema;

        public void LoadVersionInfo()
        {
            if (!AlreadyCreatedVersionSchema && !versionSchemaMigrationAlreadyRun)
            {
                Runner.Up(VersionSchemaMigration);
                versionSchemaMigrationAlreadyRun = true;
            }

            if (!AlreadyCreatedVersionTable && !versionMigrationAlreadyRun)
            {
                Runner.Up(VersionMigration);
                versionMigrationAlreadyRun = true;
            }

        
            if (!AlreadyMadeVersionUnique && !versionUniqueMigrationAlreadyRun)
            {
                this.Runner.Up(VersionUniqueMigration);
                versionUniqueMigrationAlreadyRun = true;
            }

            if (!AlreadyMadeVersionDescription && !versionDescriptionMigrationAlreadyRun)
            {
                this.Runner.Up(VersionDescriptionMigration);
                versionDescriptionMigrationAlreadyRun = true;
            }

            if(!this.AlreadyMadeModuleDescription && !this.versionModuleMigrationAlreadyRun)
            {
                this.Runner.Up(this.VersionModuleMigration);
                this.versionModuleMigrationAlreadyRun = true;
            }

            versionInfo = new NerdyMishkaVersionInfo();

            if (!this.AlreadyCreatedVersionTable) return;

            var dataSet = processor.ReadTableData(VersionTableMetaData.SchemaName, VersionTableMetaData.TableName);
            var def = this.NerdyMishkaVersionTableMetaData;
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                var version = long.Parse(row[def.ColumnName].ToString());
                var module = row[def.ModuleColumnName].ToString();

                versionInfo.AddAppliedMigration(module, version);
            }
        }

        public void RemoveVersionTable()
        {
            var expression = new DeleteTableExpression { 
                TableName = VersionTableMetaData.TableName,
                SchemaName = VersionTableMetaData.SchemaName 
            };
            
            expression.ExecuteWith(processor);

            if (OwnsVersionSchema && !string.IsNullOrEmpty(VersionTableMetaData.SchemaName))
            {
                var schemaExpression = new DeleteSchemaExpression { SchemaName = VersionTableMetaData.SchemaName };
                schemaExpression.ExecuteWith(processor);
            }
        }

        public void DeleteVersion(long version, string module)
        {
            var def = this.NerdyMishkaVersionTableMetaData;
            var expression = new DeleteDataExpression { 
                TableName = def.TableName, 
                SchemaName = def.SchemaName 
            };

            if(string.IsNullOrWhiteSpace(module))
                module = "app";
            
            expression.Rows.Add(
                new DeletionDataDefinition {
                new KeyValuePair<string, object>(def.ColumnName, version),
                new KeyValuePair<string, object>(def.ModuleColumnName, module)
            });

            expression.ExecuteWith(processor);
        }

        public void DeleteVersion(long version)
        {
            this.DeleteVersion(version, "app");
        }

    }
}