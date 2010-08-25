using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;

namespace TotalCommander.Plugin.Wfx.FileSystem
{
    public interface IFile
    {
        bool ResumeAllowed
        {
            get;
        }

        bool Exists
        {
            get;
        }

        string LocalName
        {
            get;
        }


        IEnumerator<FindData> GetFiles();


        ExecuteResult Open(TotalCommanderWindow window, ref string link);

        ExecuteResult Properties(TotalCommanderWindow window, ref string link);

        ExecuteResult ChangeMode(TotalCommanderWindow window, string mode, ref string link);

        ExecuteResult Command(TotalCommanderWindow window, string command, ref string link);


        FileOperationResult CopyTo(IFile dest, RemoteInfo info);

        FileOperationResult MoveTo(IFile dest, RemoteInfo info);

        FileOperationResult Download(string localName, CopyFlags copyFlags, RemoteInfo info);

        FileOperationResult Upload(string localName, CopyFlags copyFlags);

        bool CreateFolder(string name);

        bool Delete();


        CustomIconResult GetIcon(ref string cache, CustomIconFlag extractIconFlag, ref Icon icon);

        PreviewBitmapResult GetPreviewBitmap(ref string cache, Size size, ref Bitmap bitmap);


        bool SetAttributes(FileAttributes attributes);

        bool SetTime(DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime);
    }
}
