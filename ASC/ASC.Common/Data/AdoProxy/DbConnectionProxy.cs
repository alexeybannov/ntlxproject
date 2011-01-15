#region usings

using System;
using System.Data;

#endregion

namespace ASC.Common.Data.AdoProxy
{
    internal class DbConnectionProxy
        : IDbConnection
    {
        public readonly IDbConnection _connection;
        public readonly ProxyCtx _ctx;

        public DbConnectionProxy(IDbConnection connection, ProxyCtx ctx)
        {
            if (connection == null)
                throw new ArgumentNullException("command");
            if (ctx == null)
                throw new ArgumentNullException("ctx");
            _connection = connection;
            _ctx = ctx;
        }

        #region IDbConnection

        public IDbTransaction BeginTransaction(IsolationLevel il)
        {
            using (ExecuteHelper.Begin(
                dur => _ctx.FireExecuteEvent(this, String.Format("BeginTransaction({0})", il), dur),
                _ctx.ProfileEnabled))
                return new DbTransactionProxy(_connection.BeginTransaction(il), _ctx);
        }

        public IDbTransaction BeginTransaction()
        {
            using (ExecuteHelper.Begin(
                dur => _ctx.FireExecuteEvent(this, "BeginTransaction", dur),
                _ctx.ProfileEnabled))
                return new DbTransactionProxy(_connection.BeginTransaction(), _ctx);
        }

        public void ChangeDatabase(string databaseName)
        {
            _connection.ChangeDatabase(databaseName);
        }

        public void Close()
        {
            _connection.Close();
        }

        public string ConnectionString
        {
            get { return _connection.ConnectionString; }
            set { _connection.ConnectionString = value; }
        }

        public int ConnectionTimeout
        {
            get { return _connection.ConnectionTimeout; }
        }

        public IDbCommand CreateCommand()
        {
            return new DbCommandProxy(_connection.CreateCommand(), _ctx);
        }

        public string Database
        {
            get { return _connection.Database; }
        }

        public void Open()
        {
            using (ExecuteHelper.Begin(
                dur => _ctx.FireExecuteEvent(this, "Open", dur),
                _ctx.ProfileEnabled))
                _connection.Open();
        }

        public ConnectionState State
        {
            get { return _connection.State; }
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
                    using (ExecuteHelper.Begin(
                        dur => _ctx.FireExecuteEvent(this, "Dispose", dur),
                        _ctx.ProfileEnabled))
                        _connection.Dispose();
                }
                _disposed = true;
            }
        }

        ~DbConnectionProxy()
        {
            Dispose(false);
        }

        #endregion
    }
}