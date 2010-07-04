using System;

namespace TotalCommander.Plugin.Utils
{
	static class Int64Util
	{
		public static int High(Int64 int64)
		{
			return (int)(int64 >> sizeof(Int64));
		}

		public static int Low(Int64 int64)
		{
			int64 = (int64 << sizeof(Int64));
			return (int)(int64 >> sizeof(Int64));
		}
	}
}
