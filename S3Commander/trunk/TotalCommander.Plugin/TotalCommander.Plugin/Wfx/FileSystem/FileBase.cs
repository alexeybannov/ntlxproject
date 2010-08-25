using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace TotalCommander.Plugin.Wfx.FileSystem
{
    public class FileBase : IFile
    {
        public readonly static IFile Empty = new FileBase();


        public virtual bool ResumeAllowed
        {
            get { return false; }
        }

        public virtual bool Exists
        {
            get { return false; }
        }

        public virtual string LocalName
        {
            get { return null; }
        }

        
        public virtual IEnumerator<FindData> GetFiles()
        {
            return null;
        }


        public virtual ExecuteResult Open(TotalCommanderWindow window, ref string link)
        {
            return ExecuteResult.Default;
        }

        public virtual ExecuteResult Properties(TotalCommanderWindow window, ref string link)
        {
            return ExecuteResult.Default;
        }

        public virtual ExecuteResult ChangeMode(TotalCommanderWindow window, string mode, ref string link)
        {
            return ExecuteResult.Default;
        }

        public virtual ExecuteResult Command(TotalCommanderWindow window, string command, ref string link)
        {
            return ExecuteResult.Default;
        }


        public virtual FileOperationResult CopyTo(IFile dest, RemoteInfo info)
        {
            return FileOperationResult.Default;
        }

        public virtual FileOperationResult MoveTo(IFile dest, RemoteInfo info)
        {
            return FileOperationResult.Default;
        }

        public virtual FileOperationResult Download(string localName, CopyFlags copyFlags, RemoteInfo info)
        {
            return FileOperationResult.Default;
        }

        public virtual FileOperationResult Upload(string localName, CopyFlags copyFlags)
        {
            return FileOperationResult.Default;
        }

        public virtual bool CreateFolder(string name)
        {
            return false;
        }

        public virtual bool Delete()
        {
            return false;
        }


        public virtual CustomIconResult GetIcon(ref string cache, CustomIconFlag extractIconFlag, ref Icon icon)
        {
            return CustomIconResult.UseDefault;
        }

        public virtual PreviewBitmapResult GetPreviewBitmap(ref string cache, Size size, ref Bitmap bitmap)
        {
            return PreviewBitmapResult.None;
        }


        public bool SetAttributes(FileAttributes attributes)
        {
            return false;
        }

        public bool SetTime(DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime)
        {
            return false;
        }
    }
}
