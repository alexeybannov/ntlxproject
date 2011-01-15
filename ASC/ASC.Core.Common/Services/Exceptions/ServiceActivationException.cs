#region usings

using System;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

#endregion

namespace ASC.Core.Common.Services
{
    [Serializable]
    public class ServiceActivationException : RemotingException
    {
        public ServiceActivationException(string serviceName)
            : base(String.Format(CommonDescriptionResource.ServiceActivationException_Message_default, serviceName))
        {
        }

        public ServiceActivationException(string reason, string serviceName)
            : base(
                String.Format(
                    CommonDescriptionResource.ServiceActivationException_Message_default + "{2}{1}",
                    serviceName,
                    reason,
                    Environment.NewLine
                    )
                )
        {
        }

        public ServiceActivationException(string reason, string serviceName, Exception innerException)
            : base(
                String.Format(
                    CommonDescriptionResource.ServiceActivationException_Message_default + "{2}{1}",
                    serviceName,
                    reason,
                    Environment.NewLine
                    ),
                innerException
                )
        {
        }

        public ServiceActivationException(string serviceName, Exception innerException)
            : base(
                String.Format(CommonDescriptionResource.ServiceActivationException_Message_default, serviceName),
                innerException)
        {
        }

        protected ServiceActivationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}