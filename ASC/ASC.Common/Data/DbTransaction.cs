#region usings

using System;
using System.Data;

#endregion

namespace ASC.Common.Data
{
    public class DbTransaction : IDbTransaction
    {
        public DbTransaction(IDbTransaction transaction)
        {
            if (transaction == null) throw new ArgumentNullException("transaction");
            Transaction = transaction;
        }

        internal IDbTransaction Transaction { get; private set; }

        #region IDbTransaction Members

        public IDbConnection Connection
        {
            get { return Transaction.Connection; }
        }

        public IsolationLevel IsolationLevel
        {
            get { return Transaction.IsolationLevel; }
        }

        public void Commit()
        {
            try
            {
                Transaction.Commit();
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
                Transaction.Rollback();
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
                Transaction.Dispose();
            }
            finally
            {
                OnUnavailable();
            }
        }

        #endregion

        public event EventHandler Unavailable;

        private void OnUnavailable()
        {
            try
            {
                if (Unavailable != null) Unavailable(this, EventArgs.Empty);
            }
            catch
            {
            }
        }
    }
}