#region usings

using System;
using System.Runtime.Remoting;
using System.Runtime.Serialization;
using ASC.Core.Common.Configuration.Resources;

#endregion

namespace ASC.Core.Configuration
{
    [Serializable]
    public class ServiceRegistrationException
        : RemotingException
    {
        public ServiceRegistrationException(string message)
            : base(message)
        {
        }

        public ServiceRegistrationException(Guid serviceID, string hostName)
            : base(
                String.Format(DescriptionResource.ServiceRegistrationException_Message, hostName)
                )
        {
            ServiceID = serviceID;
            HostName = hostName;
        }

        public ServiceRegistrationException(
            SerializationInfo info,
            StreamingContext context)
            : base(info, context)
        {
            ServiceID = (Guid) info.GetValue("ServiceID", typeof (Guid));
            HostName = info.GetValue("HostName", typeof (string)) as string;
        }

        public override void GetObjectData(
            SerializationInfo info,
            StreamingContext context)
        {
            info.AddValue("ServiceID", ServiceID);
            info.AddValue("HostName", HostName);
            base.GetObjectData(info, context);
        }

        public Guid ServiceID { get; set; }

        public string HostName { get; set; }
    }
}