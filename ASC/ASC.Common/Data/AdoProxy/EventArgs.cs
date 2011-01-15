#region usings

using System;
using System.Data;

#endregion

namespace ASC.Common.Data.AdoProxy
{
    public class ExecutedEventArgs
        : EventArgs
    {
        private readonly TimeSpan _duration = TimeSpan.MinValue;
        public IDbConnection Connection;
        public string Description;
        public ExecutedObject Executed;

        public ExecutedEventArgs(ExecutedObject executed, IDbConnection connection, string description,
                                 TimeSpan? duration)
        {
            Connection = connection;
            Description = description;
            Executed = executed;
            if (duration.HasValue)
                _duration = duration.Value;
        }

        public TimeSpan Duration
        {
            get
            {
                if (_duration == TimeSpan.MinValue)
                    throw new ApplicationException("Profiling not enabled");
                return _duration;
            }
        }

        public bool ProfilingEnabled
        {
            get { return _duration != TimeSpan.MinValue; }
        }
    }

    public enum ExecutedObject
    {
        Connection,
        Command,
        Transaction
    }
}