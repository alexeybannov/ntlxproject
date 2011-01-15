#region usings

using System.Collections;
using System.Runtime.Remoting.Channels;
using ASC.Core.Common.Remoting;
using ASC.Core.Security.Authentication;
using ASC.Core.Tenants.Sink;

#endregion

namespace ASC.Core
{
    public class RemotingSubsystemConfiguration
    {
        public IClientFormatterSinkProvider GetClientFormatterSinkProvider()
        {
            var properties = new Hashtable();
            properties["typeFilterLevel"] = "Full";
            return new BinaryClientFormatterSinkProvider(properties, null);
        }

        public IServerFormatterSinkProvider GetServerFormatterSinkProvider()
        {
            var properties = new Hashtable();
            properties["typeFilterLevel"] = "Full";
            return new BinaryServerFormatterSinkProvider(properties, null);
        }

        public IClientChannelSinkProvider GetFirstClientSinkProvider()
        {
            var provider = new SecurityClientSinkProvider();
            provider.Next = new TenantClientSinkProvider();
            provider.Next.Next = GetClientFormatterSinkProvider();

            return provider;
        }

        public IServerChannelSinkProvider GetFirstServerSinkProvider()
        {
            IServerFormatterSinkProvider provider = GetServerFormatterSinkProvider();
            provider.Next = new SecurityServerSinkProvider();
            provider.Next.Next = new DebugServerSinkProvider();
            return provider;
        }
    }
}