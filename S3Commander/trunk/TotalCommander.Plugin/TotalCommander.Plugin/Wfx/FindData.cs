using System;
using System.IO;
using System.Runtime.InteropServices;
using TotalCommander.Plugin.Utils;
using TotalCommander.Plugin.Wfx.Internal;

namespace TotalCommander.Plugin.Wfx
{
    public class FindData
    {
        public static readonly FindData NotOpen = null;

        public static readonly FindData NoMoreFiles = new FindData();


        public FileAttributes Attributes
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

        public long FileSize
        {
            get;
            set;
        }

        public DateTime? CreationTime
        {
            get;
            set;
        }

        public DateTime? LastAccessTime
        {
            get;
            set;
        }

        public DateTime? LastWriteTime
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


        public FindData()
        {

        }

        public FindData(string fileName)
        {
            FileName = fileName;
        }

        public FindData(string fileName, long fileSize)
            : this(fileName)
        {
            FileSize = fileSize;
        }

        public FindData(string fileName, FileAttributes attributes)
            : this(fileName)
        {
            Attributes = attributes;
        }


        internal void CopyTo(IntPtr pFindData)
        {
            if (pFindData != IntPtr.Zero)
            {
                var findData = new FsFindData()
                {
                    FileName = FileName,
                    AlternateFileName = AlternateFileName,
                    FileAttributes = (int)Attributes,
                    FileSizeHigh = LongUtil.High(FileSize),
                    FileSizeLow = LongUtil.Low(FileSize),
                    CreationTime = DateTimeUtil.ToFileTime(CreationTime),
                    LastAccessTime = DateTimeUtil.ToFileTime(LastAccessTime),
                    LastWriteTime = DateTimeUtil.ToFileTime(LastWriteTime),
                    Reserved0 = Reserved0,
                    Reserved1 = Reserved1,
                };
                Marshal.StructureToPtr(findData, pFindData, false);
            }
        }
    }
}
