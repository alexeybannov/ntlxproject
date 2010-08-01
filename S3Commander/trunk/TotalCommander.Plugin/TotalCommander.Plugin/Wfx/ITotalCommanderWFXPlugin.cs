using System;
using System.Collections;
using System.Drawing;
using System.IO;

namespace TotalCommander.Plugin.Wfx
{
    public interface ITotalCommanderWfxPlugin
    {
        void Init(Progress progress, Logger logger, Request request);

        
        FindData FindFirst(string path, out IEnumerator enumerator);

        FindData FindNext(IEnumerator enumerator);
        
        void FindClose(IEnumerator enumerator);


        ExecuteResult FileExecute(TotalCommanderWindow window, string remoteName, string verb);

        FileOperationResult FileRenameMove(string oldName, string newName, bool move, bool overwrite, RemoteInfo ri);

        FileOperationResult FileGet(string remoteName, string localName, CopyFlags copyFlags, RemoteInfo ri);

        FileOperationResult FilePut(string localName, string remoteName, CopyFlags copyFlags);

        bool FileRemove(string remoteName);

        bool DirectoryCreate(string path);

        bool DirectoryRemove(string remoteName);


        void SetDefaultParams(DefaultParam defaultParam);

        bool SetFileAttributes(string remoteName, FileAttributes attributes);

        bool SetFileTime(string remoteName, DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime);

        
        CustomIconResult GetCustomIcon(string remoteName, CustomIconFlag extractIconFlag, out Icon icon);

        PreviewBitmapResult GetPreviewBitmap(string remoteName, Size size, out Bitmap bitmap);

        
        void StatusInfo(string remoteName, StatusInfo info, StatusOperation operation);

        bool Disconnect(string disconnectRoot);
    }
}
