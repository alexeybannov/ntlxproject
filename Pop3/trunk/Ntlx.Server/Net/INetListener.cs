using System;
using Ntlx.Server.Configuration;

namespace Ntlx.Server.Net
{
	public interface INetListener : IConfigurable
	{
		string Name
		{
			get;
			set;
		}

		bool Listening
		{
			get;
		}

		void Start();

		void Stop();

		INetConnection GetNetConnection(string id);

		event EventHandler<NetConnectionOpenEventArgs> OpenNetConnection;
	}
}
