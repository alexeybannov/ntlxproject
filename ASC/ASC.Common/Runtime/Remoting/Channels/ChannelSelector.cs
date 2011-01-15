#region usings

using System;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;
using System.Runtime.Remoting.Channels.Ipc;
using System.Runtime.Remoting.Channels.Tcp;
using ASC.Net;
using ASC.Reflection;

#endregion

namespace ASC.Runtime.Remoting.Channels
{
    public static class ChannelSelector
    {
        public static bool IsClientChannelValid(IChannelSender channel, RemoteChannelData data,
                                                object originalChannelData)
        {
            if (
                originalChannelData != null &&
                originalChannelData.GetType().Name == "CrossAppDomainData")
            {
                return false;
            }
            if (null != data)
            {
                if (!ChannelDemand.IsClientChannelValid(channel, data.ClientChannelDemand)) return false;
            }
            return true;
        }

        public static TransportType GetChannelTransportType(IChannel channel)
        {
            if (channel == null) throw new ArgumentNullException("channel");

            if (TypeHelper.IsSameOrParent(typeof (TcpClientChannel), channel.GetType()) ||
                TypeHelper.IsSameOrParent(typeof (TcpServerChannel), channel.GetType()))
            {
                return TransportType.Tcp;
            }

            if (TypeHelper.IsSameOrParent(typeof (IpcClientChannel), channel.GetType()) ||
                TypeHelper.IsSameOrParent(typeof (IpcServerChannel), channel.GetType()))
            {
                return TransportType.Ipc;
            }

            if (TypeHelper.IsSameOrParent(typeof (HttpClientChannel), channel.GetType()) ||
                TypeHelper.IsSameOrParent(typeof (HttpServerChannel), channel.GetType()))
            {
                return TransportType.Http;
            }
            throw new NotSupportedException();
        }
    }
}