#region usings

using System;
using System.Data;
using System.Text;

#endregion

namespace ASC.Common.Data.AdoProxy
{
    public class ProxyCtx
    {
        public string DatabaseId;
        public bool ProfileEnabled;
        public event Action<object, ExecutedEventArgs> ExecutedEvent;

        internal void FireExecuteEvent(IDbCommand cmd, string method, TimeSpan? duration)
        {
            string sql = DoCommandSql(cmd);
            var args = new ExecutedEventArgs(ExecutedObject.Command, cmd.Connection,
                                             String.Format("{0}:{1}", method, sql), duration);
            if (ExecutedEvent != null)
                ExecutedEvent(this, args);
        }

        internal void FireExecuteEvent(IDbConnection conn, string method, TimeSpan? duration)
        {
            var args = new ExecutedEventArgs(ExecutedObject.Connection, conn, method, duration);
            if (ExecutedEvent != null)
                ExecutedEvent(this, args);
        }

        internal void FireExecuteEvent(IDbTransaction tx, string method, TimeSpan? duration)
        {
            var args = new ExecutedEventArgs(ExecutedObject.Transaction, tx.Connection, method, duration);
            if (ExecutedEvent != null)
                ExecutedEvent(this, args);
        }

        private string DoCommandSql(IDbCommand command)
        {
            var parameters = new StringBuilder();
            foreach (IDbDataParameter p in command.Parameters)
            {
                parameters.Append(", ");
                if (!string.IsNullOrEmpty(p.ParameterName)) parameters.AppendFormat("{0}=", p.ParameterName);
                parameters.Append(p.Value == null ? "NULL" : p.Value.ToString());
            }
            return String.Format("{0} [{1}]", command.CommandText, parameters);
        }
    }
}