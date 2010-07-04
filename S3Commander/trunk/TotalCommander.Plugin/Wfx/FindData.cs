using System;
using System.IO;
using System.Runtime.InteropServices;
using TotalCommander.Plugin.Utils;
using TotalCommander.Plugin.Wfx.Internal;

namespace TotalCommander.Plugin.Wfx
{
	public class FindData
	{
		public FileAttributes Attributes
		{
			get;
			set;
		}

		public DateTime CreationTime
		{
			get;
			set;
		}

		public DateTime LastAccessTime
		{
			get;
			set;
		}

		public DateTime LastWriteTime
		{
			get;
			set;
		}

		public long FileSize
		{
			get;
			set;
		}

		public int Reserved0
		{
			get;
			set;
		}

		public int Reserved1
		{
			get;
			set;
		}

		public string FileName
		{
			get;
			set;
		}

		public string AlternateFileName
		{
			get;
			set;
		}

		internal void CopyTo(IntPtr pFindData)
		{
			if (pFindData != IntPtr.Zero){
				var findData = (WIN32_FIND_DATA)Marshal.PtrToStructure(pFindData, typeof(WIN32_FIND_DATA));
				findData.fileName = StringUtil.ToPath(FileName);
				findData.alternateFileName = StringUtil.ToPath(AlternateFileName);
				findData.fileAttributes = (int)Attributes;
				findData.dwReserved0 = Reserved0;
				findData.dwReserved1 = Reserved1;
				//findData.
			}
		}
	}
}
