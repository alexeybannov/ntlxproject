using System;
using System.Data;

namespace ASC.Common.Data.AdoProxy
{
    class ProxyCtx
    {
        public event Action<object, ExecutedEventArgs> ExecutedEvent;


        public void FireExecuteEvent(IDbCommand cmd, string method, TimeSpan duration)
        {
            if (ExecutedEvent != null) ExecutedEvent(this, new ExecutedEventArgs(string.Format("Command.{0}", method), duration, cmd));
        }

        public void FireExecuteEvent(IDbConnection conn, string method, TimeSpan duration)
        {
            if (ExecutedEvent != null) ExecutedEvent(this, new ExecutedEventArgs(string.Format("Connection.{0}", method), duration));
        }

        public void FireExecuteEvent(IDbTransaction tx, string method, TimeSpan duration)
        {
            if (ExecutedEvent != null) ExecutedEvent(this, new ExecutedEventArgs(string.Format("Transaction.{0}", method), duration));
        }
    }
}
