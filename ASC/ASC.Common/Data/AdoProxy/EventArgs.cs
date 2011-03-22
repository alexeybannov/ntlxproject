using System;
using System.Data;
using System.Text;

namespace ASC.Common.Data.AdoProxy
{
    class ExecutedEventArgs : EventArgs
    {
        public TimeSpan Duration { get; private set; }

        public string SqlMethod { get; private set; }

        public string Sql { get; private set; }

        public string SqlParameters { get; private set; }


        public ExecutedEventArgs(string method, TimeSpan duration)
            : this(method, duration, null)
        {

        }

        public ExecutedEventArgs(string method, TimeSpan duration, IDbCommand command)
        {
            SqlMethod = method;
            Duration = duration;
            if (command != null)
            {
                Sql = command.CommandText;
                
                if (0 < command.Parameters.Count)
                {
                    var stringBuilder = new StringBuilder();
                    foreach (IDbDataParameter p in command.Parameters)
                    {
                        if (!string.IsNullOrEmpty(p.ParameterName)) stringBuilder.AppendFormat("{0}=", p.ParameterName);
                        stringBuilder.AppendFormat("{0}, ", p.Value == null ? "NULL" : p.Value.ToString());
                    }
                    SqlParameters = stringBuilder.ToString(0, stringBuilder.Length - 2);
                }
            }
        }
    }
}