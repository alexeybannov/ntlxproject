using System;
using System.IO;
using System.Runtime.InteropServices;
using TotalCommander.Plugin.Utils;
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

        public DateTime? LastWriteTime
        {
            get;
            set;
        }

        public FileAttributes Attributes
        {
            get;
            set;
        }

        public bool IsDirectory
        {
            get;
            private set;
        }


        internal RemoteInfo(IntPtr ptr)
        {
            if (ptr != IntPtr.Zero)
            {
                var ri = (FsRemoteInfo)Marshal.PtrToStructure(ptr, typeof(FsRemoteInfo));
                IsDirectory = (ri.SizeHigh == -int.MaxValue && ri.SizeLow == 0);
                if (!IsDirectory) Size = LongUtil.MakeLong(ri.SizeHigh, ri.SizeLow);
                LastWriteTime = DateTimeUtil.FromFileTime(ri.LastWriteTime);
                Attributes = (FileAttributes)ri.Attributes;
            }
        }
    }
}
