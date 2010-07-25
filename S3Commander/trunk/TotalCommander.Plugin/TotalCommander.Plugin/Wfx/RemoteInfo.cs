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


        internal RemoteInfo(IntPtr ptr)
        {
            if (ptr != IntPtr.Zero)
            {
                var ri = (RemoteInfoStruct)Marshal.PtrToStructure(ptr, typeof(RemoteInfoStruct));
                Size = LongUtil.MakeLong(ri.SizeHigh, ri.SizeLow);
                LastWriteTime = DateTimeUtil.FromFileTime(ri.LastWrite);
                Attributes = (FileAttributes)ri.Attr;
            }
        }
    }
}
