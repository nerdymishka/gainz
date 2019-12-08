using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using NerdyMishka.Data;

namespace NerdyMishka.Data.Migrations
{
    public class MigrationRunner : IMigrationRunner, IDisposable 
	{
        private AkashicFactory factory;
        private bool initialized = false;

        private MigrationOptions options;

        private bool disposed = false;

		public MigrationRunner(MigrationOptions options, IMigrationHistoryService historyService)
		{
            if(options == null)
                throw new ArgumentNullException(nameof(options));

            this.options = options;
            this.factory = AkashicProviderFactory.GetFactory(this.options.ProviderName);
            this.History = historyService;
			this.MigrationAssemblies = new List<Assembly>();
		}

		[ImportMany(typeof(IMigration))]
		public IEnumerable<IMigration> Migrations { get; set; }

        [ImportMany(typeof(IDatabaseInitializer))]
        public IEnumerable<IDatabaseInitializer> DatabaseInitializers { get; set; }

		public IList<Assembly> MigrationAssemblies { get; protected set; }

        public bool DropDatabase { get; set; } = false;

		protected ISqlExecutor SqlExecutor { get; set; }

        protected IDataConnection Connection { get; set; }

		private IMigrationHistoryService History { get; set; }

		private void ComposeMigrations()
		{
			var catalog = new AggregateCatalog();

			foreach(var assembly in this.MigrationAssemblies)
				catalog.Catalogs.Add(new AssemblyCatalog(assembly));
             
			var container = new CompositionContainer(catalog);
            container.ComposeParts(this);

			if(this.Migrations.Count() == 0)
					throw new Exception("No Migrations were found");
		}

		public void Initialize()
		{
            if(!this.initialized)
            {
                this.ComposeMigrations();

                if(this.Connection == null)
                    this.Connection = factory.CreateConnection(this.options.ConnectionString);

                if(this.DatabaseInitializers != null && this.DatabaseInitializers.Count() > 0)
                {
                    foreach(var initializer in this.DatabaseInitializers) 
                    {
                        initializer.Factory = this.factory;
                        initializer.Connection = this.Connection;
                        initializer.Create(this.DropDatabase);
                        if(initializer is IDisposable)
                        {
                            ((IDisposable)initializer).Dispose();
                        }
                    }
                }                
            }

            if(this.Connection == null)
                this.Connection = factory.CreateConnection(this.options.ConnectionString);

            if(this.Connection.State == DataConnectionState.Closed)
                this.Connection.Open();

			IRdbmsMigrationHistoryService rdbms = null;

			if(this.History is IRdbmsMigrationHistoryService)
			{
				rdbms = (IRdbmsMigrationHistoryService)this.History;
				rdbms.SqlExecutor = this.Connection.BeginTransaction();
				if(!rdbms.HasStore()) {
					rdbms.CreateStore();
				}
				// commit;
				rdbms.SqlExecutor.Dispose();
				rdbms.SqlExecutor = null;
			}

            if(this.SqlExecutor == null)
            {
                this.Connection.Open();
                this.SqlExecutor = this.Connection.BeginTransaction();
            }

			if(rdbms != null)
				rdbms.SqlExecutor = this.SqlExecutor;
		}

		public void StepToVersion(int steps, string category = null)
		{
			if (steps < 0)
				this.StepDownTo(Math.Abs(steps), category);
			else
				this.StepUpTo(steps, category);
		}

        public void Run(string category = null) => this.RunToVersion(long.MaxValue, category);

		public void RunToVersion(long version, string category = null)
		{
            this.Initialize();
			
			if (version < 1)
			{
				var list = (from o in this.Migrations
				 where o.Category == category
					 && o.Version > -1
				 orderby o.Version descending
				 select o).ToList();

				var first = list.FirstOrDefault();
				

				this.RunUpTo(first.Version, category);

				return;
			}

            long currentVersion = this.FetchCurrentVersion(category);
			if (version > currentVersion)
				this.RunUpTo(version, category, currentVersion);
			else
				this.RunDownTo(version, category, currentVersion);
		}

		private class Sorter : IComparer<IMigration>
		{
			public int Compare(IMigration x, IMigration y)
			{
				return x.Version.CompareTo(y.Version);
			}
		}

		protected virtual long FetchCurrentVersion(string category)
		{
		    return this.History.GetCurrentVersion(category);
		}

		private void StepUpTo(int step, string category = null)
		{
	
			this.Execute((list, currentVersion, prefix) =>
			{
				int count = 0;

				foreach (var item in list)
				{
					if (count == step)
						break;

					if (item.Version > currentVersion)
					{
						item.SqlExecutor = this.SqlExecutor;
						item.Up();
                        this.History.AddVersion(item);
						count++;
					}
				}
			}, ListSortDirection.Ascending, category);
		}

		private void StepDownTo(int step, string category = null)
		{
			this.Execute((list, currentVersion, prefix) =>
			{
				int count = 0;

				foreach (var item in list)
				{
					if (count == step)
						break;

					if (item.Version <= currentVersion)
					{
						item.SqlExecutor = this.SqlExecutor;
						item.Down();
						this.History.RemoveVersion(item.Version);
						count++;
					}
				}

			}, ListSortDirection.Descending, category);
		}

		private void RunDownTo(long version, string category = null, long currentMigrationVersion = -1)
		{
			Execute((list, currentVersion, prefix) =>
			{
				foreach (var item in list)
				{
					if (item.Version <= currentVersion && item.Version >= version)
					{
						item.SqlExecutor = this.SqlExecutor;
						item.Down();
						this.History.RemoveVersion(item.Version);
					}
				}

                
			}, ListSortDirection.Descending, category, currentMigrationVersion);
		}

		private void RunUpTo(long version, string category = null, long currentMigrationVersion =  -1)
		{
			this.Execute((list, currentVersion, prefix) =>
			{
				foreach (var item in list)
				{
					if (item.Version > currentVersion && item.Version <= version)
					{
						item.SqlExecutor = this.SqlExecutor;
						item.Up();
						this.History.AddVersion(item);
					}
				}

                
			}, ListSortDirection.Ascending, category, currentMigrationVersion);
		}

        public void Dispose()
        {
            if(!this.disposed)
            {
                this.SqlExecutor?.Dispose();
                this.SqlExecutor = null;
                this.Connection?.Dispose();
                this.Connection = null;
                this.disposed = true;
            }  
        }

		private void Execute(
            Action<IList<IMigration>, long, string> block, 
            ListSortDirection direction = ListSortDirection.Ascending, 
            string category = null, 
            long currentVersion = -1)
		{
			List<IMigration> list;
			if (direction == ListSortDirection.Ascending)
			{

				list = (from o in this.Migrations
						where o.Category == category
							&& o.Version > currentVersion
						orderby o.Version ascending
						select o).ToList();
			}
			else
			{
				list = (from o in this.Migrations
						where o.Category == category
							&& o.Version < currentVersion
						orderby o.Version descending
						select o).ToList();
			}

            this.Initialize();
        
            if(currentVersion == -1)
			    currentVersion = FetchCurrentVersion(category);    

			block(list, currentVersion, category);

            this.SqlExecutor.Dispose();
            this.SqlExecutor = null;
		}
	}
}