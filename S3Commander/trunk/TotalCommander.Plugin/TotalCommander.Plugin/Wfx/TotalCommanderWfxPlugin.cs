using System;
using System.Collections;
using System.Drawing;
using System.IO;

namespace TotalCommander.Plugin.Wfx
{
    public abstract class TotalCommanderWfxPlugin : ITotalCommanderWfxPlugin
    {
        public Progress Progress
        {
            get;
            private set;
        }

        public Logger Logger
        {
            get;
            private set;
        }

        public Request Request
        {
            get;
            private set;
        }

        public Version PluginInterfaceVersion
        {
            get;
            private set;
        }

        public string IniFilePath
        {
            get;
            private set;
        }


        public void Init(Progress progress, Logger logger, Request request)
        {
            Progress = progress;
            Logger = logger;
            Request = request;
            Init();
        }

        public virtual void Init()
        {

        }

        public abstract FindData FindFirst(string path, out IEnumerator enumerator);

        public abstract FindData FindNext(IEnumerator enumerator);

        public virtual void FindClose(IEnumerator enumerator)
        {

        }


        public ExecuteResult FileExecute(TotalCommanderWindow window, string remoteName, string verb)
        {
            if (string.IsNullOrEmpty(verb)) return ExecuteResult.Error;
            var command = 0 < verb.IndexOf(' ') ? verb.Substring(verb.IndexOf(' ')).Trim() : string.Empty;
            switch (verb.ToLower().Substring(0, 1))
            {
                case "o": return ExecuteOpen(window, remoteName);
                case "p": return ExecuteProperties(window, remoteName);
                case "c": return ExecuteChMode(window, remoteName, command);
                case "q": return ExecuteCommand(window, remoteName, command);
            }
            return ExecuteResult.Default;
        }

        public virtual ExecuteResult ExecuteProperties(TotalCommanderWindow window, string remoteName)
        {
            return ExecuteResult.Default;
        }

        public virtual ExecuteResult ExecuteChMode(TotalCommanderWindow window, string remoteName, string mode)
        {
            return ExecuteResult.Default;
        }

        public virtual ExecuteResult ExecuteCommand(TotalCommanderWindow window, string remoteName, string command)
        {
            return ExecuteResult.Default;
        }

        public virtual ExecuteResult ExecuteOpen(TotalCommanderWindow window, string remoteName)
        {
            return ExecuteResult.Default;
        }


        public virtual FileOperationResult FileGet(string remoteName, string localName, CopyFlags copyFlags, RemoteInfo ri)
        {
            return FileOperationResult.Default;
        }

        public virtual FileOperationResult FilePut(string localName, string remoteName, CopyFlags copyFlags)
        {
            return FileOperationResult.Default;
        }

        public FileOperationResult FileRenameMove(string oldName, string newName, bool move, bool overwrite, RemoteInfo ri)
        {
            return ri.IsDirectory ?
                DirectoryRename(oldName, newName, overwrite, ri) :
                move ?
                FileMove(oldName, newName, overwrite, ri) :
                FileCopy(oldName, newName, overwrite, ri);
        }

        public virtual FileOperationResult FileMove(string oldName, string newName, bool overwrite, RemoteInfo ri)
        {
            return FileOperationResult.Default;
        }

        public virtual FileOperationResult FileCopy(string oldName, string newName, bool overwrite, RemoteInfo ri)
        {
            return FileOperationResult.Default;
        }

        public virtual FileOperationResult DirectoryRename(string oldName, string newName, bool overwrite, RemoteInfo ri)
        {
            return FileOperationResult.Default;
        }


        public virtual bool FileRemove(string remoteName)
        {
            return false;
        }

        public virtual bool DirectoryCreate(string path)
        {
            return false;
        }

        public virtual bool DirectoryRemove(string remoteName)
        {
            return false;
        }



        public virtual CustomIconResult GetCustomIcon(string remoteName, CustomIconFlag extractIconFlag, out Icon icon)
        {
            icon = null;
            return CustomIconResult.UseDefault;
        }

        public virtual PreviewBitmapResult GetPreviewBitmap(string remoteName, Size size, out Bitmap bitmap)
        {
            bitmap = null;
            return PreviewBitmapResult.None;
        }

        
        public void SetDefaultParams(DefaultParam defaultParam)
        {
            PluginInterfaceVersion = defaultParam.PluginInterfaceVersion;
            IniFilePath = defaultParam.DefaultIniFileName;
        }

        public virtual bool SetFileAttributes(string remoteName, FileAttributes attributes)
        {
            return false;
        }

        public virtual bool SetFileTime(string remoteName, DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime)
        {
            return false;
        }


        public virtual bool Disconnect(string disconnectRoot)
        {
            return false;
        }

        public virtual void StatusInfo(string remoteName, StatusInfo info, StatusOperation operation)
        {

        }
    }
}
