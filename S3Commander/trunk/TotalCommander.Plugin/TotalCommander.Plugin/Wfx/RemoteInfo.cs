﻿using System;
using System.IO;
using System.Runtime.InteropServices;
using TotalCommander.Plugin.Utils;
using TotalCommander.Plugin.Wfx.Internal;

namespace TotalCommander.Plugin.Wfx
{
    /// <summary>
    /// <see cref="RemoteInfo"/> is passed to <see cref="ITotalCommanderWfxPlugin.FileGet"/> and 
    /// <see cref="ITotalCommanderWfxPlugin.FileRenameMove"/>. It contains details about the remote file being copied.
    /// </summary>
    /// <remarks>
    /// This struct is passed to <see cref="ITotalCommanderWfxPlugin.FileGet"/> and <see cref="ITotalCommanderWfxPlugin.FileRenameMove"/>
    /// to make it easier for the plugin to copy the file. You can of course also ignore this parameter.
    /// </remarks>
    /// <seealso cref="ITotalCommanderWfxPlugin.FileGet"/>
    /// <seealso cref="ITotalCommanderWfxPlugin.FileRenameMove"/>
    public class RemoteInfo
    {
        /// <summary>
        /// The remote file size. Useful for a progress indicator.
        /// </summary>
        public long Size
        {
            get;
            set;
        }

        /// <summary>
        /// Time stamp of the remote file - should be copied with the file.
        /// </summary>
        public DateTime? LastWriteTime
        {
            get;
            set;
        }

        /// <summary>
        /// Attributes of the remote file - should be copied with the file.
        /// </summary>
        public FileAttributes Attributes
        {
            get;
            set;
        }

        /// <summary>
        /// Indicates that the remote file is directory.
        /// </summary>
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
