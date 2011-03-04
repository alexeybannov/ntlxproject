using System;
using System.Data;

namespace ASC.Common.Data.AdoProxy
{
    class DbCommandProxy : IDbCommand
    {
        private readonly IDbCommand command;
        private readonly ProxyCtx context;
        private bool disposed;


        public DbCommandProxy(IDbCommand command, ProxyCtx ctx)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (ctx == null) throw new ArgumentNullException("ctx");

            this.command = command;
            context = ctx;
        }


        public void Cancel()
        {
            command.Cancel();
        }

        public string CommandText
        {
            get { return command.CommandText; }
            set { command.CommandText = value; }
        }

        public int CommandTimeout
        {
            get { return command.CommandTimeout; }
            set { command.CommandTimeout = value; }
        }

        public CommandType CommandType
        {
            get { return command.CommandType; }
            set { command.CommandType = value; }
        }

        public IDbConnection Connection
        {
            get { return new DbConnectionProxy(command.Connection, context); }
            set { command.Connection = value is DbConnectionProxy ? value : new DbConnectionProxy(value, context); }
        }

        public IDbDataParameter CreateParameter()
        {
            return command.CreateParameter();
        }

        public int ExecuteNonQuery()
        {
            using (ExecuteHelper.Begin(dur => context.FireExecuteEvent(this, "ExecuteNonQuery", dur)))
            {
                return command.ExecuteNonQuery();
            }
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            using (ExecuteHelper.Begin(dur => context.FireExecuteEvent(this, string.Format("ExecuteReader({0})", behavior), dur)))
            {
                return command.ExecuteReader(behavior);
            }
        }

        public IDataReader ExecuteReader()
        {
            using (ExecuteHelper.Begin(dur => context.FireExecuteEvent(this, "ExecuteReader", dur)))
            {
                return command.ExecuteReader();
            }
        }

        public object ExecuteScalar()
        {
            using (ExecuteHelper.Begin(dur => context.FireExecuteEvent(this, "ExecuteScalar", dur)))
            {
                return command.ExecuteScalar();
            }
        }

        public IDataParameterCollection Parameters
        {
            get { return command.Parameters; }
        }

        public void Prepare()
        {
            command.Prepare();
        }

        public IDbTransaction Transaction
        {
            get { return command.Transaction; }
            set { command.Transaction = value is DbTransactionProxy ? ((DbTransactionProxy)value).transaction : value; }
        }

        public UpdateRowSource UpdatedRowSource
        {
            get { return command.UpdatedRowSource; }
            set { command.UpdatedRowSource = value; }
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    command.Dispose();
                }
                disposed = true;
            }
        }

        ~DbCommandProxy()
        {
            Dispose(false);
        }
    }
}