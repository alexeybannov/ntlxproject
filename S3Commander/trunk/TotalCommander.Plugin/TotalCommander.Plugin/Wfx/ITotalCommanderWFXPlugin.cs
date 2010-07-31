using System;
using System.Collections;
using System.Drawing;
using System.IO;

namespace TotalCommander.Plugin.Wfx
{
    public interface ITotalCommanderWfxPlugin
    {
        void SetDefaultParams(DefaultParam defaultParam);

        void Init(Progress progress, Logger logger, Request request);

        
        FindData FindFirst(string path, out IEnumerator enumerator);

        FindData FindNext(IEnumerator enumerator);
        
        void FindClose(IEnumerator enumerator);


        ExecuteResult ExecuteFile(MainWindow mainWindow, string remoteName, string verb);

        FileOperationResult RenameMoveFile(string oldName, string newName, bool move, bool overWrite, RemoteInfo ri);

        bool RemoveFile(string remoteName);

        bool CreateDirectory(string path);

        bool RemoveDirectory(string remoteName);


        bool SetAttributes(string remoteName, FileAttributes attributes);

        bool SetTime(string remoteName, DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime);

        ExtractIconResult ExtractCustomIcon(string remoteName, ExtractIconFlag extractIconFlag, out Icon icon);

        PreviewBitmapResult GetPreviewBitmap(string remoteName, Size size, out Bitmap bitmap);

        void StatusInfo(string remoteName, StatusInfo info, StatusOperation operation);

        bool Disconnect(string disconnectRoot);


        FileOperationResult GetFile(string remoteName, string localName, CopyFlags copyFlags, RemoteInfo ri);

        FileOperationResult PutFile(string localName, string remoteName, CopyFlags copyFlags);
    }
}
