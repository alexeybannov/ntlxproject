using System;
using System.Collections;

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

		public string DefaultIniFileName
		{
			get;
			private set;
		}


		public void SetDefaultParams(DefaultParam defaultParam)
		{
			PluginInterfaceVersion = defaultParam.PluginInterfaceVersion;
			DefaultIniFileName = defaultParam.DefaultIniFileName;
		}

		public void Init(Progress progress, Logger logger, Request request)
		{
			Progress = progress;
			Logger = logger;
			Request = request;
			OnInit();
		}

		protected virtual void OnInit()
		{

		}

        public virtual FindData FindFirst(string path, out IEnumerator enumerator)
		{
			enumerator = null;
			return null;
		}

        public virtual FindData FindNext(IEnumerator enumerator)
		{
			return null;
		}

		public virtual void FindClose(IEnumerator enumerator)
		{

		}


		public virtual ExecuteResult ExecuteFile(MainWindow mainWindow, string remoteName, string verb)
		{
			return ExecuteResult.Error;
		}

		public virtual FileOperationResult RenameMoveFile(string oldName, string newName, bool move, bool overWrite, RemoteInfo ri)
		{
			return FileOperationResult.NotSupported;
		}

        public virtual bool RemoveFile(string remoteName)
		{
			return false;
		}

        public virtual bool CreateDirectory(string path)
		{
			return false;
		}

        public virtual bool RemoveDirectory(string remoteName)
		{
			return false;
		}


        public virtual FileOperationResult GetFile(string remoteName, string localName, CopyFlags copyFlags, RemoteInfo ri)
        {
            return FileOperationResult.NotSupported;
        }

        public virtual FileOperationResult PutFile(string localName, string remoteName, CopyFlags copyFlags)
        {
            return FileOperationResult.NotSupported;
        }

        public virtual bool SetAttributes(string remoteName, System.IO.FileAttributes attributes)
        {
            return false;
        }

        public virtual bool SetTime(string remoteName, DateTime? creationTime, DateTime? lastAccessTime, DateTime? lastWriteTime)
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

        public virtual ExtractIconResult ExtractCustomIcon(string remoteName, ExtractIconFlag extractIconFlag, out System.Drawing.Icon icon)
        {
            icon = null;
            return ExtractIconResult.UseDefault;
        }

        public virtual PreviewBitmapResult GetPreviewBitmap(string remoteName, System.Drawing.Size size, out System.Drawing.Bitmap bitmap)
        {
            bitmap = null;
            return PreviewBitmapResult.None;
        }
    }
}
