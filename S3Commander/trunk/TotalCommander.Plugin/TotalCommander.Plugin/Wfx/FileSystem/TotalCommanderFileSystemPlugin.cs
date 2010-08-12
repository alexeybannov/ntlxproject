using System;
using System.Collections;
using TotalCommander.Plugin;
using TotalCommander.Plugin.Wfx;

namespace TotalCommander.Plugin.Wfx.FileSystem
{
    public abstract class TotalCommanderFileSystemPlugin : ITotalCommanderWfxPlugin
    {
        private IFileSystem fileSystem;
        private FileSystemContext context;


        public abstract string PluginName
        {
            get;
        }

        public virtual bool TemporaryPanelPlugin
        {
            get;
            protected set;
        }

        public virtual BackgroundFlags BackgroundSupport
        {
            get;
        }

        public void Init(int pluginNumber, Progress progress, Log log, Request request)
        {
            context = new FileSystemContext();
            context.PluginName = PluginName;
            context.PluginNumber = pluginNumber;
            context.Progress = progress;
            context.Log = log;
            context.Request = request;
            context.TemporaryPanelPlugin = IsLinksToLocalFiles(
        }

                
        public FindData FindFirst(string path, out IEnumerator enumerator)
        {
            enumerator = null;

            var file = fileSystem.ResolvePath(path);
            if (file != null)
            {
                enumerator = file.GetChildren().GetEnumerator();
                return FindNext(enumerator);
            }
            return FindData.NotOpen;
        }

        public FindData FindNext(IEnumerator enumerator)
        {
            return enumerator != null && enumerator.MoveNext() ?
                ToFindData((IFile)enumerator.Current) :
                FindData.NoMoreFiles;
        }

        public void FindClose(IEnumerator enumerator)
        {
            var disposable = enumerator as IDisposable;
            if (disposable != null) disposable.Dispose();
        }


        private FindData ToFindData(IFile file)
        {
            return new FindData(file.Name)
            {
                Attributes = file.Attributes,
                FileSize = file.FileSize,
                LastWriteTime = file.LastWriteTime
            };
        }

        public ExecuteResult FileExecute(TotalCommanderWindow window, ref string remoteName, string verb)
        {
            throw new NotImplementedException();
        }

        public FileOperationResult FileRenameMove(string oldName, string newName, bool move, bool overwrite, RemoteInfo ri)
        {
            throw new NotImplementedException();
        }

        public FileOperationResult FileGet(string remoteName, ref string localName, CopyFlags copyFlags, RemoteInfo ri)
        {
            throw new NotImplementedException();
        }

        public FileOperationResult FilePut(string localName, ref string remoteName, CopyFlags copyFlags)
        {
            throw new NotImplementedException();
        }

        public bool FileRemove(string remoteName)
        {
            throw new NotImplementedException();
        }

        public bool DirectoryCreate(string path)
        {
            throw new NotImplementedException();
        }

        public bool DirectoryRemove(string remoteName)
        {
            throw new NotImplementedException();
        }

        public void SetDefaultParams(DefaultParam defaultParam)
        {
            throw new NotImplementedException();
        }

        public bool SetFileAttributes(string remoteName, System.IO.FileAttributes attributes)
        {
            throw new NotImplementedException();
        }

        public bool SetFileTime(string remoteName, DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime)
        {
            throw new NotImplementedException();
        }

        public void SetPasswordStore(Password password)
        {
            throw new NotImplementedException();
        }

        public BackgroundFlags GetBackgroundFlags()
        {
            throw new NotImplementedException();
        }

        public CustomIconResult GetCustomIcon(ref string remoteName, CustomIconFlag extractIconFlag, out System.Drawing.Icon icon)
        {
            throw new NotImplementedException();
        }

        public PreviewBitmapResult GetPreviewBitmap(ref string remoteName, System.Drawing.Size size, out System.Drawing.Bitmap bitmap)
        {
            throw new NotImplementedException();
        }

        public bool IsLinksToLocalFiles()
        {
            throw new NotImplementedException();
        }

        public string GetLocalName(string remoteName)
        {
            throw new NotImplementedException();
        }

        public void StatusInfo(string remoteName, StatusOrigin origin, StatusOperation operation)
        {
            throw new NotImplementedException();
        }

        public bool Disconnect(string disconnectRoot)
        {
            throw new NotImplementedException();
        }
    }
}
