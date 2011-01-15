#region usings

using System;
using System.Runtime.Remoting;
using ASC.Common.Services;
using ASC.Common.Utils;
using ASC.Core.Common.Services;
using ASC.Runtime.Remoting.Channels;

#endregion

namespace ASC.Core.Common.Remoting
{
    internal sealed class RemotingActivator
    {
        public IService Activate(InternalServiceInfoCache.ServiceInfoEx srvInfo)
        {
            LogHolder.Log("ASC.Core.Common").InfoFormat("activating service {0}[instanceID={2}] at {1}...",
                                                        srvInfo.Name, srvInfo.ServiceEndPoint.ServiceUri,
                                                        srvInfo.InstanceID);
            if (srvInfo == null) throw new ArgumentNullException("srvInfoEx");
            var data = new RemoteChannelData
                           {
                               ClientChannelDemand = ServiceDescriber.RequestChannelDemand(srvInfo)
                           };
            if (srvInfo.ServiceEndPoint == null || data.ClientChannelDemand == null) throw new ArgumentNullException();
            var channel = WorkContext.RemotingSubsystem.StartupClientChannel(
                srvInfo.ServiceEndPoint.TransportType,
                data.ClientChannelDemand,
                CoreContext.Configuration.RemotingSubsystemConfiguration.GetFirstClientSinkProvider()) as
                          IChannelWithServices;
            if (channel != null)
            {
                channel.AttachService(srvInfo.InstanceID);
            }
            object objRef = RemotingServices.Connect(srvInfo.ServiceType, srvInfo.ServiceEndPoint.ServiceUri, data);
            var service = objRef as IService;
            if (service == null)
            {
                throw new ServiceActivationException(
                    string.Format(CommonDescriptionResource.ServiceActivationException_Message_IncorrectType,
                                  srvInfo.ServiceType.FullName, service.GetType().FullName), srvInfo.Name);
            }
            return service;
        }
    }
}