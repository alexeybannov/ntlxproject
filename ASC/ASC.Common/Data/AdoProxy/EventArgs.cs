using System;
using System.Data;

namespace ASC.Common.Data.AdoProxy
{
    class ExecutedEventArgs : EventArgs
    {
        public TimeSpan Duration { get; private set; }

        public string Description { get; private set; }

        public ExecutedObject Executed { get; private set; }


        public ExecutedEventArgs(ExecutedObject executed, string description, TimeSpan duration)
        {
            Description = description;
            Executed = executed;
            Duration = duration;
        }
    }

    public enum ExecutedObject
    {
        Connection,
        Command,
        Transaction
    }
}