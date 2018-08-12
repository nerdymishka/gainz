using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace NerdyMishka.Data
{
    public class DataTransaction : IDataTransaction
    {
        private DbTransaction transaction;
        private DataConnection connection;
        private SqlDialect dialect;
        private bool autoCommit = false;
        private bool completed = false;
        private IsolationLevel level = IsolationLevel.Unspecified;
        private CommandBehavior commandBehavior = CommandBehavior.Default;


        public DataTransaction(
            DbTransaction dbTransaction,
            SqlDialect dialect)
        {
            this.transaction = dbTransaction;
            this.dialect = dialect;
            this.connection = new DataConnection(dbTransaction.Connection, dialect);
            this.autoCommit = false;
        }


        public DataTransaction(
            DataConnection connection,
            SqlDialect dialect = null,
            IsolationLevel level = IsolationLevel.Unspecified,
            bool autoCommit = false)
        {
           

            this.connection = connection;
            if (this.connection.State == DataConnectionState.Open)
            {
                this.transaction = this.connection.InnerConnection.BeginTransaction(level);
                autoCommit = false;
            }
               

            this.dialect = dialect;
            this.level = level;
            this.autoCommit = autoCommit;
        }

        public void SetAutoCommit()
        {
            this.autoCommit = true;
        }

        public void OnNext(SqlStatementContext data)
        {
            if(this.connection.State != DataConnectionState.Open)
            {
                this.autoCommit = true;
                this.connection.Open();
                this.transaction = connection.InnerConnection.BeginTransaction(level);
            }

            if(this.transaction == null)
            {
                this.transaction = connection.InnerConnection.BeginTransaction(level);
            }

            if (data == null)
                return;

            if (data.Command == null)
                data.Command = this.CreateCommand();

            switch (data.Type)
            {
                case SqlStatementContext.ParameterSetType.Array:
                    data.Command.Configure(data.Query, data.ParameterArray);
                    break;
                case SqlStatementContext.ParameterSetType.DbParameters:
                    data.Command.Configure(data.Query, data.DbParameters);
                    break;
                case SqlStatementContext.ParameterSetType.Hashtable:
                    data.Command.Configure(data.Query, data.Hastable);
                    break;
                case SqlStatementContext.ParameterSetType.KeyValue:
                    data.Command.Configure(data.Query, data.Parameters);
                    break;
                default:
                    data.Command.Configure(data.Query);
                    break;
            }
        }

        public void OnError(Exception ex)
        {
            this.Rollback();
        }

        public void OnCompleted()
        {
            if (this.autoCommit)
            {
                this.Commit();
            }
        }

        public DataConnection Connection => this.connection;



        public SqlDialect SqlDialect => this.dialect;

        IDataConnection IDataTransaction.Connection => this.connection;

        public void Commit()
        {
            if(!this.completed)
            {
                this.completed = true;
                this.transaction.Commit();
            }
        }

        public IDataCommand CreateCommand() {
            var cmd = this.transaction.Connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.Transaction = this.transaction;
            cmd.Connection = this.transaction.Connection;
            return new DataCommand(cmd, commandBehavior);
        }

        public IDataCommand CreateCommand(CommandBehavior commandBehavior)
        {
            var cmd = this.transaction.Connection.CreateCommand();
            cmd.CommandType = CommandType.Text;
            cmd.Transaction = this.transaction;
            cmd.Connection = this.transaction.Connection;
            return new DataCommand(cmd, commandBehavior);
        }

        public void Dispose()
        {

            this.Dispose(true);
            GC.SuppressFinalize(this);

        }

        protected void Dispose(bool dispose)
        {
            if (dispose)
            {
                this.transaction.Dispose();
                this.transaction = null;
            }
        }

        ~DataTransaction()
        {
            this.Dispose(false);
        }

        public void Rollback()
        {
            if(!this.completed)
            {
                this.completed = true;
                this.transaction.Rollback();
            }    
        }

        public SqlBuilder CreateSqlBuilder()
        {
            return new SqlBuilder(this.SqlDialect);
        }
    }
}
