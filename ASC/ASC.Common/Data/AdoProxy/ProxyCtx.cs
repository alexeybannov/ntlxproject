using System;
using System.Data;
using System.Text;

namespace ASC.Common.Data.AdoProxy
{
    class ProxyCtx
    {
        public event Action<object, ExecutedEventArgs> ExecutedEvent;


        public void FireExecuteEvent(IDbCommand cmd, string method, TimeSpan duration)
        {
            var a = new ExecutedEventArgs(ExecutedObject.Command, string.Format("{0}:     {1}", method, DoCommandSql(cmd)), duration);
            if (ExecutedEvent != null) ExecutedEvent(this, a);
        }

        public void FireExecuteEvent(IDbConnection conn, string method, TimeSpan duration)
        {
            var args = new ExecutedEventArgs(ExecutedObject.Connection, method, duration);
            if (ExecutedEvent != null) ExecutedEvent(this, args);
        }

        public void FireExecuteEvent(IDbTransaction tx, string method, TimeSpan duration)
        {
            var args = new ExecutedEventArgs(ExecutedObject.Transaction, method, duration);
            if (ExecutedEvent != null) ExecutedEvent(this, args);
        }

        private string DoCommandSql(IDbCommand command)
        {
            var parameters = new StringBuilder();
            foreach (IDbDataParameter p in command.Parameters)
            {
                if (!string.IsNullOrEmpty(p.ParameterName)) parameters.AppendFormat("{0}=", p.ParameterName);
                parameters.Append(p.Value == null ? "NULL" : p.Value.ToString());
                parameters.Append(", ");
            }
            if (0 < parameters.Length) parameters.Remove(parameters.Length - 2, 2);
            return string.Format("{0} [{1}]", command.CommandText, parameters);
        }
    }
}
