using System;
using System.Runtime.Serialization;

namespace NXmlConnector
{
    public class NXmlConnectorException : Exception
    {
        public NXmlConnectorException(string message)
            : base(message)
        {

        }

        public NXmlConnectorException(string message, Exception innerException)
            : base(message, innerException)
        {

        }

        protected NXmlConnectorException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
    }
}
