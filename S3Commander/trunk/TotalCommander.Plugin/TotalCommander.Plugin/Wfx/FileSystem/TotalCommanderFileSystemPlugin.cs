﻿using System;
using System.Collections;
using System.Drawing;
using System.IO;

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
            protected set;
        }


        public void Init(int pluginNumber, Progress progress, Log log, Request request)
        {
            TemporaryPanelPlugin = false;
            BackgroundSupport = BackgroundFlags.NotSupported;

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

            fileSystem = CreateFileSystem();
            if (fileSystem != null) fileSystem.Initialize(context);
        }

        protected abstract IFileSystem CreateFileSystem();


        public FindData FindFirst(string path, out IEnumerator enumerator)
        {
            enumerator = fileSystem != null ? fileSystem.GetFiles(path) : null;
            return FindNext(enumerator);
        }

        public FindData FindNext(IEnumerator enumerator)
        {
            if (enumerator == null) return FindData.NotOpen;
            return enumerator.MoveNext() ? ((IFile)enumerator.Current).GetFileInfo() : FindData.NoMoreFiles;
        }

        public void FindClose(IEnumerator enumerator)
        {
            var disposable = enumerator as IDisposable;
            if (disposable != null) disposable.Dispose();
        }


        public ExecuteResult FileExecute(TotalCommanderWindow window, ref string remoteName, string verb)
        {
            var file = ResolvePath(remoteName);
            switch ((verb ?? " ").ToLower().Substring(0, 1))
            {
                case "o": return file.Open(window, ref remoteName);
                case "p": return file.Properties(window, ref remoteName);
                //case "c": return ExecuteChMode(window, ref remoteName, command);
                //case "q": return ExecuteCommand(window, ref remoteName, command);
                default: return ExecuteResult.Default;
            }
        }


        public FileOperationResult FileRenameMove(string oldName, string newName, bool move, bool overwrite, RemoteInfo ri)
        {
            return FileOperationResult.Default;
        }

        public FileOperationResult FileGet(string remoteName, ref string localName, CopyFlags copyFlags, RemoteInfo ri)
        {
            return FileOperationResult.Default;
        }

        public FileOperationResult FilePut(string localName, ref string remoteName, CopyFlags copyFlags)
        {
            return FileOperationResult.Default;
        }

        public bool FileRemove(string remoteName)
        {
            return ResolvePath(remoteName).Remove();
        }


        public bool DirectoryCreate(string path)
        {
            if (string.IsNullOrEmpty(path)) return false;
            var pos = path.Trim('\\').LastIndexOf('\\');
            if (0 < pos)
            {
                return ResolvePath(path.Substring(0, pos)).CreateFolder(path.Substring(pos + 1));
            }
            return false;
        }

        public bool DirectoryRemove(string remoteName)
        {
            return ResolvePath(remoteName).Remove();
        }


        public bool SetFileAttributes(string remoteName, FileAttributes attributes)
        {
            return false;
        }

        public bool SetFileTime(string remoteName, DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime)
        {
            return false;
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
            return context.TemporaryPanelPlugin;
        }

        public string GetLocalName(string remoteName)
        {
            return null;
        }


        public void StatusInfo(string remoteName, StatusOrigin origin, StatusOperation operation)
        {
            if (fileSystem != null) fileSystem.StatusInfo(remoteName, origin, operation);
        }

        public bool Disconnect(string disconnectRoot)
        {
            return fileSystem != null ? fileSystem.Disconnect(disconnectRoot) : false;
        }


        private IFile ResolvePath(string path)
        {
            return (fileSystem != null ? fileSystem.ResolvePath(path) : null) ?? File.Empty;
        }
    }
}
