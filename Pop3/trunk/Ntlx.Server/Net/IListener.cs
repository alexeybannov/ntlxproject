using System;
using Ntlx.Server.Configuration;
using Ntlx.Server.Formatters;

namespace Ntlx.Server.Net
{
	public interface IListener : IConfigurable
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

		IConnection GetConnection(string id);

		event EventHandler<ConnectionOpenEventArgs> OpenConnection;
	}
}
