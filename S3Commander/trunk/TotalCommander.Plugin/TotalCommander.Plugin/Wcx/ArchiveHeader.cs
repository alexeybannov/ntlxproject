using System;
using System.Runtime.InteropServices;
using System.IO;
using TotalCommander.Plugin.Utils;

namespace TotalCommander.Plugin.Wcx
{
    public class ArchiveHeader
    {
        public string ArchiveName
        {
            get;
            set;
        }

        public string FileName
        {
            get;
            set;
        }

        public FileAttributes FileAttributes
        {
            get;
            set;
        }

        public long PackedSize
        {
            get;
            set;
        }

        public long UnpackedSize
        {
            get;
            set;
        }

        public int FileCRC
        {
            get;
            set;
        }

        public DateTime FileTime
        {
            get;
            set;
        }


        internal void CopyTo(IntPtr ptr)
        {
            if (ptr != IntPtr.Zero)
            {
                var data = new ArchiveHeaderStruct
                {
                    ArchiveName = ArchiveName,
                    FileName = FileName,
                    FileAttr = (int)FileAttributes,
                    FileCRC = FileCRC,
                    FileTime = GetFileTime(),
                    PackSizeHigh = (uint)LongUtil.High(PackedSize),
                    PackSize = (uint)LongUtil.Low(PackedSize),
                    UnpSizeHigh = (uint)LongUtil.High(UnpackedSize),
                    UnpSize = (uint)LongUtil.Low(UnpackedSize),
                };
                Marshal.StructureToPtr(data, ptr, false);
            }
        }

        private int GetFileTime()
        {
            var year = FileTime.Year;
            return 1980 <= year && year <= 2100 ?
                (year - 1980) << 25 | FileTime.Month << 21 | FileTime.Day << 16 | FileTime.Hour << 11 | FileTime.Minute << 5 | FileTime.Second / 2 :
                0;
        }


        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        public struct ArchiveHeaderStruct
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Win32.MAX_PATH)]
            public string ArchiveName;

            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = Win32.MAX_PATH)]
            public string FileName;

            public int Flags;

            public uint PackSize;
            public uint PackSizeHigh;

            public uint UnpSize;
            public uint UnpSizeHigh;

            public int HostOS;

            public int FileCRC;

            public int FileTime;

            public int UnpVer;

            public int Method;

            public int FileAttr;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string CmtBuf;
            public int CmtBufSize;
            public int CmtSize;
            public int CmtState;

            [MarshalAs(UnmanagedType.ByValArray, SizeConst = Win32.MAX_PATH)]
            public byte[] Reserved;
        }
    }
}
