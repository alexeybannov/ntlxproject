using System;
using System.Data;

namespace ASC.Common.Data.AdoProxy
{
    class DbTransactionProxy : IDbTransaction
    {
        private bool disposed;
        private readonly ProxyCtx context;
        public readonly IDbTransaction transaction;

        public DbTransactionProxy(IDbTransaction transaction, ProxyCtx ctx)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            if (ctx == null) throw new ArgumentNullException("ctx");

            this.transaction = transaction;
            context = ctx;
        }


        public void Commit()
        {
            using (ExecuteHelper.Begin(dur => context.FireExecuteEvent(this, "Commit", dur)))
            {
                transaction.Commit();
            }
        }

        public IDbConnection Connection
        {
            get { return transaction.Connection; }
        }

        public IsolationLevel IsolationLevel
        {
            get { return transaction.IsolationLevel; }
        }

        public void Rollback()
        {
            using (ExecuteHelper.Begin(dur => context.FireExecuteEvent(this, "Rollback", dur)))
            {
                transaction.Rollback();
            }
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
                    transaction.Dispose();
                }
                disposed = true;
            }
        }

        ~DbTransactionProxy()
        {
            Dispose(false);
        }
    }
}