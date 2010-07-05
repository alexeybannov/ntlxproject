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
            if (pFindData != IntPtr.Zero)
            {
                var findData = new WIN32_FIND_DATA();
                findData.fileName = StringUtil.ToPath(FileName);
                findData.alternateFileName = StringUtil.First(AlternateFileName, 12);
                findData.fileAttributes = (int)Attributes;
                findData.dwReserved0 = Reserved0;
                findData.dwReserved1 = Reserved1;
                findData.creationTimeHigh = High(ToFileTime(CreationTime));
                findData.creationTimeLow = Low(ToFileTime(CreationTime));
                findData.lastAccessTimeHigh = High(ToFileTime(LastAccessTime));
                findData.lastAccessTimeLow = Low(ToFileTime(LastAccessTime));
                findData.lastWriteTimeHigh = High(ToFileTime(LastWriteTime));
                findData.lastWriteTimeLow = Low(ToFileTime(LastWriteTime));
                findData.nFileSizeHigh = High(FileSize);
                findData.nFileSizeLow = Low(FileSize);
                Marshal.StructureToPtr(findData, pFindData, false);
            }
        }

        private long ToFileTime(DateTime dateTime)
        {
            return dateTime != DateTime.MinValue ? dateTime.ToFileTime() : PluginConst.NO_FILETIME;
        }

        private int High(Int64 int64)
        {
            return (int)(int64 >> 8 * sizeof(Int64) / 2);
        }

        private int Low(Int64 int64)
        {
            return High(int64 << 8 * sizeof(Int64) / 2);
        }
    }
}
