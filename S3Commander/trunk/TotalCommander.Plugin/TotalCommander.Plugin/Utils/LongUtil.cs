using System;

namespace TotalCommander.Plugin.Utils
{
	static class LongUtil
	{
        public static int High(Int64 int64)
        {
            return (int)(int64 >> 32);
        }

        public static int Low(Int64 int64)
        {
            return High(int64 << 32);
        }

        public static int High(UInt64 int64)
        {
            return High((Int64)int64);
        }

        public static int Low(UInt64 int64)
        {
            return Low((Int64)int64);
        }

        public static long MakeLong(int high, int low)
        {
            return (((long)high) << 32) + low;
        }
    }
}
