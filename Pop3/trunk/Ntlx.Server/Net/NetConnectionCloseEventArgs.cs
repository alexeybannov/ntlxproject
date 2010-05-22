
namespace Ntlx.Server.Net
{
	public class NetConnectionCloseEventArgs : NetConnectionOpenEventArgs
	{
		public NetConnectionCloseReason Reason
		{
			get;
			private set;
		}

		public NetConnectionCloseEventArgs(INetConnection connection, NetConnectionCloseReason reason)
			: base(connection)
		{
			Reason = reason;
		}
	}
}
