#region usings

using System;

#endregion

namespace ASC.Notify
{
    public class NotifyException
        : ApplicationException
    {
        public NotifyException(string message)
            : base(message)
        {
        }

        public NotifyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}