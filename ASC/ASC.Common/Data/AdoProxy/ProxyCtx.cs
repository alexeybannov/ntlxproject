using System;
using System.Data;

namespace ASC.Common.Data.AdoProxy
{
    class ProxyCtx
    {
        public event Action<object, ExecutedEventArgs> ExecutedEvent;


        public void FireExecuteEvent(IDbCommand cmd, string method, TimeSpan duration)
        {
            OnExecutedEvent(new ExecutedEventArgs("Command." + method, duration, cmd));
        }

        public void FireExecuteEvent(IDbConnection conn, string method, TimeSpan duration)
        {
            OnExecutedEvent(new ExecutedEventArgs("Connection." + method, duration));
        }

        public void FireExecuteEvent(IDbTransaction tx, string method, TimeSpan duration)
        {
            OnExecutedEvent(new ExecutedEventArgs("Transaction." + method, duration));
        }


        private void OnExecutedEvent(ExecutedEventArgs a)
        {
            var ev = ExecutedEvent;
            if (ev != null) ev(this, a);
        }
    }
}
