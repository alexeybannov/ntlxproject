using System;
using System.Runtime.Serialization;

namespace ASC.Core.Tenants
{
    [Serializable]
    public class TenantTooShortException : Exception
    {
        public TenantTooShortException(string message)
            : base(message)
        {
        }

        protected TenantTooShortException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class TenantIncorrectCharsException : Exception
    {
        public TenantIncorrectCharsException(string message)
            : base(message)
        {
        }

        protected TenantIncorrectCharsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }

    [Serializable]
    public class TenantAlreadyExistsException : Exception
    {
        public TenantAlreadyExistsException(string alias)
            : base(string.Format("Address '{0}' busy.", alias))
        {
        }

        protected TenantAlreadyExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}