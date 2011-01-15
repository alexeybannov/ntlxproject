#region usings

using System;
using System.Net.Security;
using System.Runtime.Remoting.Channels;
using System.Security.Principal;
using ASC.Net;

#endregion

namespace ASC.Runtime.Remoting.Channels
{
    [Serializable]
    public class ChannelDemand
    {
        private readonly string _ChannelName = "default_channel_name";
        private readonly bool _ChannelNameSetted;
        private readonly ProtectionLevel _ProtectionLevel = ProtectionLevel.None;
        private readonly bool _Secure;
        private readonly TokenImpersonationLevel _TokenImpersonationLevel = TokenImpersonationLevel.Identification;

        public ChannelDemand()
        {
        }

        public ChannelDemand(bool secure)
        {
            _Secure = secure;
            if (secure)
                _ProtectionLevel = ProtectionLevel.EncryptAndSign;
        }

        public ChannelDemand(ProtectionLevel protectionLevel)
        {
            _Secure = true;
            _ProtectionLevel = protectionLevel;
        }

        public ChannelDemand(bool secure, string channelName)
            : this(secure)
        {
            _ChannelName = channelName;
            _ChannelNameSetted = true;
        }

        public ChannelDemand(ProtectionLevel protectionLevel, string channelName)
            : this(protectionLevel)
        {
            _ChannelName = channelName;
            _ChannelNameSetted = true;
        }

        public ChannelDemand(ProtectionLevel protectionLevel, TokenImpersonationLevel impersonationLevel,
                             string channelName)
            : this(protectionLevel, channelName)
        {
            _TokenImpersonationLevel = impersonationLevel;
        }

        public bool Secure
        {
            get { return _Secure; }
        }

        public string ChannelName
        {
            get { return _ChannelName; }
        }

        public ProtectionLevel ProtectionLevel
        {
            get { return _ProtectionLevel; }
        }

        public TokenImpersonationLevel TokenImpersonationLevel
        {
            get { return _TokenImpersonationLevel; }
        }

        public bool HasDefaultName
        {
            get { return !_ChannelNameSetted; }
        }

        public static bool IsClientChannelValid(IChannelSender channel, ChannelDemand demand)
        {
            if (null != channel && null != demand)
            {
                var secureChannel = channel as ISecurableChannel;
                if (demand.Secure &&
                    (null != secureChannel && !secureChannel.IsSecured))
                    return false;
                if (!demand.Secure && secureChannel != null && secureChannel.IsSecured)
                    return false;
                if (!demand.HasDefaultName && !ChannelNameEqual(channel, demand.ChannelName))
                    return false;
            }
            return true;
        }

        public static bool IsServerChannelValid(IChannelReceiver channel, ChannelDemand demand)
        {
            if (null != channel && null != demand)
            {
                var secureChannel = channel as ISecurableChannel;
                if (demand.Secure &&
                    (null != secureChannel && !secureChannel.IsSecured))
                    return false;
                if (!demand.Secure && secureChannel != null && secureChannel.IsSecured)
                    return false;
                if (!demand.HasDefaultName && !ChannelNameEqual(channel, demand.ChannelName))
                    return false;
            }
            return true;
        }

        public static string BuildChannelName(TransportType transportType, int port, string channelName)
        {
            if (port == 0)
                return
                    String.Format(
                        "{0}#{1}",
                        transportType,
                        channelName
                        );
            else
                return String.Format(
                    "{0}:{1}#{2}",
                    transportType,
                    port,
                    channelName
                    );
        }

        public static bool ChannelNameEqual(IChannel channel, string channelName)
        {
            if (channel == null) return false;
            return ChannelNameEqual(channel.ChannelName, channelName);
        }

        public static bool ChannelNameEqual(string realName, string reqName)
        {
            if (reqName == null || realName == null) return false;
            string name = GetChannelName(realName);
            return name.ToLower()
                   ==
                   reqName.ToLower();
        }

        public static string GetChannelName(string realName)
        {
            if (String.IsNullOrEmpty(realName))
                throw new ArgumentNullException("realName");
            return realName.Substring(realName.IndexOf('#') + 1);
        }
    }
}