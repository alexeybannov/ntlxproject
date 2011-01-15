#region usings

using System;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

#endregion

namespace ASC.Core.Common.Services
{
    [Serializable]
    public class ServiceInstanceNotFoundException : RemotingException
    {
        private readonly Guid _ServiceInstanceID;

        public ServiceInstanceNotFoundException(Guid serviceInstanceID)
            :
                base(
                String.Format(CommonDescriptionResource.ServiceInstanceNotFoundException_Message, serviceInstanceID))
        {
            _ServiceInstanceID = serviceInstanceID;
        }

        public ServiceInstanceNotFoundException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
        }

        public Guid ServiceInstanceID
        {
            get { return _ServiceInstanceID; }
        }
    }
}