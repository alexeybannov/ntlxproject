using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Ntlx.Server.Formatters
{
	public interface IFormatter
	{
		void WriteMessage(byte[] buffer, int offset, int count);

		event EventHandler DeserializedMessage;

		byte[] SerializeMessage(object obj);
	}
}
