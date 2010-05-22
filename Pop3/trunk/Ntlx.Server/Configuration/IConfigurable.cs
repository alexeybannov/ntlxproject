using System.Collections.Generic;

namespace Ntlx.Server.Configuration
{
	public interface IConfigurable
	{
		void Configure(IDictionary<string, string> properties);
	}
}
