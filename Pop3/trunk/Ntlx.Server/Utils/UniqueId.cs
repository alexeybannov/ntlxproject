using System.Security.Cryptography;
using System.Text;

namespace Ntlx.Server.Utils
{
	public static class UniqueId
	{
		public static string Generate()
		{
			return Generate(4);
		}

		public static string Generate(int length)
		{
			var buffer = new byte[length];
			RandomNumberGenerator.Create().GetBytes(buffer);
			var sb = new StringBuilder();
			foreach (var b in buffer) sb.Append(b.ToString("x2"));
			return sb.ToString();
		}
	}
}
