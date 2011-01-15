#region usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Remoting.Channels;
using System.Security.Principal;
using ASC.Common.Utils;
using ASC.Net;
using ASC.Runtime.Remoting.Channels;
using ASC.Runtime.Remoting.Channels.Http;
using ASC.Runtime.Remoting.Channels.Ipc;
using ASC.Runtime.Remoting.Channels.Tcp;
using TransportType = ASC.Net.TransportType;

#endregion

namespace ASC.Runtime.Remoting
{
    public sealed class RemotingSubsystem
    {
        public IChannelSender StartupClientChannel(TransportType transportType, ChannelDemand channelDemand,
                                                   IClientChannelSinkProvider clientSinkProvider)
        {
            if (transportType != TransportType.Http && transportType != TransportType.Ipc &&
                transportType != TransportType.Tcp)
            {
                throw new NotSupportedException();
            }
            if (channelDemand == null) channelDemand = new ChannelDemand();
            IChannelSender resultChannel = null;
            int port = 0;
            string channelName = ChannelDemand.BuildChannelName(transportType, port, channelDemand.ChannelName);
            IChannel channel = GetChannelByName(channelName);
            if (channel != null)
            {
                resultChannel = channel as IChannelSender;
                if (resultChannel != null)
                {
                    if (ChannelDemand.IsClientChannelValid(resultChannel, channelDemand)) return resultChannel;
                    if (channelDemand.HasDefaultName && port == 0)
                    {
                        channelName += "_" + Guid.NewGuid();
                    }
                }
            }
            var properties = new Hashtable();
            properties["name"] = channelName;
            properties["priority"] = 99;
            if (channelDemand.Secure)
            {
                properties["protectionLevel"] = channelDemand.ProtectionLevel;
                properties["tokenImpersonationLevel"] = channelDemand.TokenImpersonationLevel;
            }
            LogHolder.Log("ASC.Common").InfoFormat("RemotingSubsystem: starting up client channel {0}...", channelName);
            switch (transportType)
            {
                case TransportType.Ipc:
                    resultChannel = new IpcClientChannelEx(properties, clientSinkProvider);
                    break;
                case TransportType.Tcp:
                    resultChannel = new TcpClientChannelEx(properties, clientSinkProvider);
                    break;
                case TransportType.Http:
                    resultChannel = new HttpClientChannelEx(properties, clientSinkProvider);
                    break;
            }
            ChannelServices.RegisterChannel(resultChannel, channelDemand.Secure);
            return resultChannel;
        }

        public IChannelReceiver StartupServerChannel(TransportType transportType, int port, ChannelDemand channelDemand,
                                                     IServerChannelSinkProvider serverSinkProvider)
        {
            if (transportType != TransportType.Http && transportType != TransportType.Ipc &&
                transportType != TransportType.Tcp)
            {
                throw new NotSupportedException();
            }
            if (channelDemand == null) channelDemand = new ChannelDemand();
            IChannelReceiver resultChannel = null;
            string channelName = ChannelDemand.BuildChannelName(transportType, port, channelDemand.ChannelName);
            IChannel channel = GetChannelByName(channelName);
            if (channel != null)
            {
                resultChannel = channel as IChannelReceiver;
                if (resultChannel != null)
                {
                    TransportType transType = ChannelSelector.GetChannelTransportType(resultChannel);
                    if (transType == transportType && ChannelDemand.IsServerChannelValid(resultChannel, channelDemand))
                    {
                        return resultChannel;
                    }
                }
            }
            var properties = new Hashtable();
            properties["name"] = channelName;
            properties["port"] = port;
            properties["portName"] = new ConnectionHostEntry(new IPAddress[] {}, port).IpcPort;
            properties["priority"] = 99;
            if (channelDemand.Secure)
            {
                properties["protectionLevel"] = channelDemand.ProtectionLevel;
                properties["impersonate"] = false;
                properties["secure"] = "true";
            }
            var sid = new SecurityIdentifier(WellKnownSidType.WorldSid, null);
            properties["authorizedGroup"] = sid.Translate(typeof (NTAccount)).Value;

            LogHolder.Log("ASC.Common").InfoFormat("RemotingSubsystem: starting up server channel {0}...", channelName);
            switch (transportType)
            {
                case TransportType.Ipc:
                    resultChannel = new IpcServerChannelEx(properties, serverSinkProvider);
                    break;
                case TransportType.Tcp:
                    resultChannel = new TcpServerChannelEx(properties, serverSinkProvider);
                    break;
                case TransportType.Http:
                    resultChannel = new HttpServerChannelEx(properties, serverSinkProvider);
                    break;
            }
            ChannelServices.RegisterChannel(resultChannel, channelDemand.Secure);
            return resultChannel;
        }

        public void DeattachServiceFromChannels(Guid identity, bool closeChannels)
        {
            if (identity == Guid.Empty) throw new ArgumentNullException("identity");
            var channelsToUnregister = new List<IChannel>();
            foreach (IChannel channel in ChannelServices.RegisteredChannels)
            {
                var channelWithServices = channel as IChannelWithServices;
                if (channelWithServices != null)
                {
                    channelWithServices.DeattachService(identity);
                    if (channelWithServices.Services.Count == 0 && closeChannels)
                    {
                        channelsToUnregister.Add(channel);
                    }
                }
            }
            foreach (IChannel channel in channelsToUnregister)
            {
                LogHolder.Log("ASC.Common").InfoFormat(
                    "RemotingSubsystem unregister channel \"{0}\" because no one service attahced...",
                    channel.ChannelName);
                ChannelServices.UnregisterChannel(channel);
            }
        }

        private IChannel GetChannelByName(string name)
        {
            foreach (IChannel ch in ChannelServices.RegisteredChannels)
            {
                if (ch.ChannelName == name) return ch;
            }
            return null;
        }
    }
}