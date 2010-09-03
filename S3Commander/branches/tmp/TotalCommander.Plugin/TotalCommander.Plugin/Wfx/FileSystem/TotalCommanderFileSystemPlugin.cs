using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.IO;

namespace TotalCommander.Plugin.Wfx.FileSystem
{
    public abstract class TotalCommanderFileSystemPlugin : ITotalCommanderWfxPlugin
    {
        private FileSystemContext context;

        private IFileSystem fileSystem;

        private IFileSystem FileSystem
        {
            get
            {
                if (fileSystem == null) fileSystem = CreateFileSystem(context);
                return fileSystem;
            }
        }


        public abstract string PluginName
        {
            get;
        }

        public virtual bool TemporaryPanelPlugin
        {
            get { return false; }
        }

        public virtual BackgroundFlags BackgroundSupport
        {
            get { return BackgroundFlags.NotSupported; }
        }


        public void Init(int pluginNumber, Progress progress, Log log, Request request)
        {
            context = new FileSystemContext()
            {
                PluginName = PluginName,
                PluginNumber = pluginNumber,
                Progress = progress,
                Log = log,
                Request = request,
                TemporaryPanelPlugin = TemporaryPanelPlugin,
                BackgroundSupport = BackgroundSupport
            };
        }

        public void SetDefaultParams(DefaultParam defaultParam)
        {
            context.PluginInterfaceVersion = defaultParam.PluginInterfaceVersion;
            context.IniFilePath = defaultParam.DefaultIniFileName;
        }

        public void SetPasswordStore(Password password)
        {
            context.Password = password;
        }


        protected abstract IFileSystem CreateFileSystem(FileSystemContext context);


        public FindData FindFirst(string path, out IEnumerator enumerator)
        {
            enumerator = ResolvePath(path).GetFiles();
            return FindNext(enumerator);
        }

        public FindData FindNext(IEnumerator enumerator)
        {
            if (enumerator == null) return FindData.NotOpen;
            return enumerator.MoveNext() ? (FindData)enumerator.Current : FindData.NoMoreFiles;
        }

        public void FindClose(IEnumerator enumerator)
        {
            var disposable = enumerator as IDisposable;
            if (disposable != null) disposable.Dispose();
        }


        public ExecuteResult FileExecute(TotalCommanderWindow window, ref string remoteName, string verb)
        {
            var file = ResolvePath(remoteName);
            switch ((verb ?? "  ").ToLower().Substring(0, 1))
            {
                case "o": return file.Open(window, ref remoteName);
                case "p": return file.Properties(window, ref remoteName);
                case "c": return file.ChangeMode(window, verb.Substring(verb.IndexOf(' ') + 1), ref remoteName);
                case "q": return file.Command(window, verb.Substring(verb.IndexOf(' ') + 1), ref remoteName);
                default: return ExecuteResult.Default;
            }
        }


        public FileOperationResult FileRenameMove(string oldName, string newName, bool move, bool overwrite, RemoteInfo ri)
        {
            var oldFile = ResolvePath(oldName);
            var newFile = ResolvePath(newName);

            if (!overwrite && newFile.Exists)
            {
                return newFile.ResumeAllowed ? FileOperationResult.ExistsResumeAllowed : FileOperationResult.Exists;
            }

            return move ? oldFile.MoveTo(newFile, ri) : oldFile.CopyTo(newFile, ri);
        }

        public FileOperationResult FileGet(string remoteName, ref string localName, CopyFlags copyFlags, RemoteInfo ri)
        {
            return ResolvePath(remoteName).Download(localName, copyFlags, ri);
        }

        public FileOperationResult FilePut(string localName, ref string remoteName, CopyFlags copyFlags)
        {
            return ResolvePath(remoteName).Upload(localName, copyFlags);
        }

        public bool FileRemove(string remoteName)
        {
            return ResolvePath(remoteName).DeleteFile();
        }


        public bool DirectoryCreate(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            var pos = path.TrimEnd('\\').LastIndexOf('\\');
            if (0 <= pos)
            {
                return ResolvePath(path.Substring(0, pos)).CreateFolder(path.Substring(pos + 1));
            }
            return false;
        }

        public bool DirectoryRemove(string remoteName)
        {
            return ResolvePath(remoteName).DeleteFolder();
        }


        public bool SetFileAttributes(string remoteName, FileAttributes attributes)
        {
            return ResolvePath(remoteName).SetAttributes(attributes);
        }

        public bool SetFileTime(string remoteName, DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime)
        {
            return ResolvePath(remoteName).SetTime(creationTime, lastAccessTime, lastWriteTime);
        }


        public BackgroundFlags GetBackgroundFlags()
        {
            return context.BackgroundSupport;
        }

        public CustomIconResult GetCustomIcon(ref string remoteName, CustomIconFlag extractIconFlag, out Icon icon)
        {
            icon = null;
            return ResolvePath(remoteName).GetIcon(ref remoteName, extractIconFlag, ref icon);
        }

        public PreviewBitmapResult GetPreviewBitmap(ref string remoteName, Size size, out Bitmap bitmap)
        {
            bitmap = null;
            return ResolvePath(remoteName).GetPreviewBitmap(ref remoteName, size, ref bitmap);
        }

        public bool IsLinksToLocalFiles()
        {
            return TemporaryPanelPlugin;
        }

        public string GetLocalName(string remoteName)
        {
            return TemporaryPanelPlugin ? ResolvePath(remoteName).LocalName : null;
        }


        public void StatusInfo(string remoteName, StatusOrigin origin, StatusOperation operation)
        {
            if (FileSystem != null) FileSystem.OperationInfo(remoteName, origin, operation);
        }

        public bool Disconnect(string disconnectRoot)
        {
            return FileSystem != null ? FileSystem.Disconnect(disconnectRoot) : false;
        }

        public virtual void OnError(Exception error)
        {

        }


        private IFile ResolvePath(string path)
        {
            return (FileSystem != null ? FileSystem.ResolvePath(path) : null) ?? FileBase.Empty;
        }
    }
}
