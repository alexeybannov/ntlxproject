using System;
using System.Data;

namespace ASC.Common.Data
{
    class DbTransaction : IDbTransaction
    {
        private IDbTransaction transaction;


        public DbTransaction(IDbTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            this.transaction = transaction;
        }


        public IDbConnection Connection
        {
            get { return transaction.Connection; }
        }

        public IsolationLevel IsolationLevel
        {
            get { return transaction.IsolationLevel; }
        }


        public void Commit()
        {
            try
            {
                transaction.Commit();
            }
            finally
            {
                OnUnavailable();
            }
        }

        public void Rollback()
        {
            try
            {
                transaction.Rollback();
            }
            finally
            {
                OnUnavailable();
            }
        }

        public void Dispose()
        {
            try
            {
                transaction.Dispose();
            }
            finally
            {
                OnUnavailable();
            }
        }

        public event EventHandler Unavailable;


        private void OnUnavailable()
        {
            try
            {
                if (Unavailable != null) Unavailable(this, EventArgs.Empty);
            }
            catch { }
        }
    }
}