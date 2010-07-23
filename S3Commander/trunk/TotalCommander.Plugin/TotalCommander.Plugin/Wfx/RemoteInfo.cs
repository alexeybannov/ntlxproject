using System;
using System.IO;
using System.Runtime.InteropServices;
using TotalCommander.Plugin.Wfx.Internal;

namespace TotalCommander.Plugin.Wfx
{
	public class RemoteInfo
	{
		public long Size
		{
			get;
			set;
		}

		public DateTime LastWriteTime
		{
			get;
			set;
		}

		public FileAttributes Attributes
		{
			get;
			set;
		}


		internal static RemoteInfo FromPtr(IntPtr ptr)
		{
			var remoteInfo = new RemoteInfo();
			if (ptr != IntPtr.Zero)
			{
				var ri = (RemoteInfoStruct)Marshal.PtrToStructure(ptr, typeof(RemoteInfoStruct));
				remoteInfo.Size = MakeLong(ri.SizeHigh, ri.SizeLow);
				if (ri.LastWriteHigh != 0 && ri.LastWriteLow != 0)
				{
					remoteInfo.LastWriteTime = DateTime.FromFileTime(MakeLong(ri.LastWriteLow, ri.LastWriteHigh));
				}
				remoteInfo.Attributes = (FileAttributes)remoteInfo.Attributes;
			}
			return remoteInfo;
		}

		private static long MakeLong(int high, int low)
		{
			return (((long)high) << (8 * sizeof(Int32))) + low;
		}
	}
}
