#region usings

using System;
using System.Data;

#endregion

namespace ASC.Common.Data.AdoProxy
{
    internal class DbTransactionProxy
        : IDbTransaction
    {
        public readonly ProxyCtx _ctx;
        public readonly IDbTransaction _transaction;

        public DbTransactionProxy(IDbTransaction transaction, ProxyCtx ctx)
        {
            if (transaction == null)
                throw new ArgumentNullException("command");
            if (ctx == null)
                throw new ArgumentNullException("ctx");
            _transaction = transaction;
            _ctx = ctx;
        }

        #region IDbTransaction

        public void Commit()
        {
            using (ExecuteHelper.Begin(
                dur => _ctx.FireExecuteEvent(this, "commit", dur),
                _ctx.ProfileEnabled))
                _transaction.Commit();
        }

        public IDbConnection Connection
        {
            get { return _transaction.Connection; }
        }

        public IsolationLevel IsolationLevel
        {
            get { return _transaction.IsolationLevel; }
        }

        public void Rollback()
        {
            using (ExecuteHelper.Begin(
                dur => _ctx.FireExecuteEvent(this, "rollback", dur),
                _ctx.ProfileEnabled))
                _transaction.Rollback();
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
                    _transaction.Dispose();
                }
                _disposed = true;
            }
        }

        ~DbTransactionProxy()
        {
            Dispose(false);
        }

        #endregion
    }
}