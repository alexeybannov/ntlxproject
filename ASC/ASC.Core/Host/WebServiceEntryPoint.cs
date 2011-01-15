using System;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Http;

namespace ASC.Core.Host
{
    class WebServiceEntryPoint : MarshalByRefObject
    {
        HttpChannel _httpChannel = null;

        internal void Start(int portNum, string uri)
        {
            StartupHTTPSOAP(portNum, uri);
        }

        internal void Stop()
        {
            ShutdownHTTPSOAP();
        }


        internal void StartupHTTPSOAP(int portNum, string uri)
        {
            _httpChannel = new HttpChannel(portNum);
            ChannelServices.RegisterChannel(_httpChannel, false);
            RemotingConfiguration.RegisterWellKnownServiceType(
                typeof(AscWSEntryPoint),
                uri,
                WellKnownObjectMode.SingleCall
            );
            //var or = RemotingServices.Marshal(this, uri, typeof(IAscWSEntryPoint));
        }

        internal void ShutdownHTTPSOAP()
        {
            if (_httpChannel != null)
            {
                //RemotingServices.Disconnect(this);
                ChannelServices.UnregisterChannel(_httpChannel);
                _httpChannel = null;
            }
        }

        public override object InitializeLifetimeService()
        {
            return null;
        }
    }
}
