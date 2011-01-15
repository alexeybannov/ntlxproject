#region usings

using System;
using System.Data;

#endregion

namespace ASC.Common.Data.AdoProxy
{
    internal class DbCommandProxy : IDbCommand
    {
        public readonly IDbCommand _command;
        public readonly ProxyCtx _ctx;

        public DbCommandProxy(IDbCommand command, ProxyCtx ctx)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (ctx == null) throw new ArgumentNullException("ctx");
            _command = command;
            _ctx = ctx;
        }

        #region IDbCommand

        public void Cancel()
        {
            _command.Cancel();
        }

        public string CommandText
        {
            get { return _command.CommandText; }
            set { _command.CommandText = value; }
        }

        public int CommandTimeout
        {
            get { return _command.CommandTimeout; }
            set { _command.CommandTimeout = value; }
        }

        public CommandType CommandType
        {
            get { return _command.CommandType; }
            set { _command.CommandType = value; }
        }

        public IDbConnection Connection
        {
            get { return new DbConnectionProxy(_command.Connection, _ctx); }
            set
            {
                if (value is DbConnectionProxy)
                    _command.Connection = value;
                else
                    _command.Connection = new DbConnectionProxy(value, _ctx);
            }
        }

        public IDbDataParameter CreateParameter()
        {
            return _command.CreateParameter();
        }

        public int ExecuteNonQuery()
        {
            using (ExecuteHelper.Begin(
                dur => _ctx.FireExecuteEvent(this, "ExecuteNonQuery", dur),
                _ctx.ProfileEnabled))
                return _command.ExecuteNonQuery();
        }

        public IDataReader ExecuteReader(CommandBehavior behavior)
        {
            using (ExecuteHelper.Begin(
                dur => _ctx.FireExecuteEvent(this, String.Format("ExecuteReader({0})", behavior), dur),
                _ctx.ProfileEnabled))
                return _command.ExecuteReader(behavior);
        }

        public IDataReader ExecuteReader()
        {
            using (ExecuteHelper.Begin(
                dur => _ctx.FireExecuteEvent(this, "ExecuteReader", dur),
                _ctx.ProfileEnabled))
                return _command.ExecuteReader();
        }

        public object ExecuteScalar()
        {
            using (ExecuteHelper.Begin(
                dur => _ctx.FireExecuteEvent(this, "ExecuteScalar", dur),
                _ctx.ProfileEnabled))
                return _command.ExecuteScalar();
        }

        public IDataParameterCollection Parameters
        {
            get { return _command.Parameters; }
        }

        public void Prepare()
        {
            _command.Prepare();
        }

        public IDbTransaction Transaction
        {
            get { return _command.Transaction; }
            set
            {
                if (value is DbTransactionProxy)
                    _command.Transaction = ((DbTransactionProxy) value)._transaction;
                else
                    _command.Transaction = value;
            }
        }

        public UpdateRowSource UpdatedRowSource
        {
            get { return _command.UpdatedRowSource; }
            set { _command.UpdatedRowSource = value; }
        }

        #endregion

        #region IDisposable

        private bool _disposed;

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposeManaged)
        {
            if (!_disposed)
            {
                if (disposeManaged)
                {
                    _command.Dispose();
                }
                _disposed = true;
            }
        }

        ~DbCommandProxy()
        {
            Dispose(false);
        }

        #endregion
    }
}