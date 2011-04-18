using System;
using System.Runtime.InteropServices;

namespace TotalCommander.Plugin.Wcx
{
    public class OpenArchiveData
    {
        private readonly IntPtr ptr;
        private OpenArchiveDataStruct data;


        public string ArchiveName
        {
            get { return data.ArchiveName; }
        }

        public OpenArchiveMode Mode
        {
            get { return (OpenArchiveMode)data.Mode; }
        }

        public ArchiveResult Result
        {
            get
            {
                return (ArchiveResult)data.Result;
            }
            set
            {
                if (ptr != IntPtr.Zero)
                {
                    data.Result = (int)value;
                    Marshal.StructureToPtr(data, ptr, false);
                }
            }
        }


        internal OpenArchiveData(IntPtr ptr)
        {
            this.ptr = ptr;
            if (ptr != IntPtr.Zero)
            {
                data = (OpenArchiveDataStruct)Marshal.PtrToStructure(ptr, typeof(OpenArchiveDataStruct));
            }
        }

        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        struct OpenArchiveDataStruct
        {
            [MarshalAs(UnmanagedType.LPWStr)]
            public string ArchiveName;

            public Int32 Mode;

            public Int32 Result;

            [MarshalAs(UnmanagedType.LPWStr)]
            public string CommentBuffer;

            public Int32 CommentBufferSize;

            public Int32 CommentSize;

            public Int32 CommentState;
        }
    }
}
