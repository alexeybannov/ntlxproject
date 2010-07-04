
namespace TotalCommander.Plugin.Utils
{
	static class StringUtil
	{
		public static string First(string str, int len)
		{
			if (str == null) return null;
			return 0 < len && len < str.Length ? str.Substring(0, len) : str;
		}

		public static string ToPath(string str)
		{
			return First(str, PluginConst.MAX_PATH - 1);
		}
	}
}
