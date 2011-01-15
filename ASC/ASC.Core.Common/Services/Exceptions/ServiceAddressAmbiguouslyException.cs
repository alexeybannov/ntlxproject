#region usings

using System;
using System.Runtime.Remoting;

#endregion

namespace ASC.Core.Common.Services
{
    [Serializable]
    public class ServiceAddressAmbiguouslyException : RemotingException
    {
        public ServiceAddressAmbiguouslyException(string serviceName)
            : base(String.Format(CommonDescriptionResource.ServiceAddressAmbiguouslyException_Message, serviceName))
        {
        }
    }
}