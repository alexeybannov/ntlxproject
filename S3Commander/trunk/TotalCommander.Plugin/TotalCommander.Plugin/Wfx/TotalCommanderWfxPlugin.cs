using System;

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

		public virtual bool FindFirst(string path, FindData findData, out object enumerator)
		{
			enumerator = null;
			return false;
		}

		public virtual bool FindNext(object enumerator, FindData findData)
		{
			return false;
		}

		public virtual void FindClose(object enumerator)
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

		public bool RemoveFile(string remoteName)
		{
			return false;
		}

		public bool CreateDirectory(string path)
		{
			return false;
		}

		public bool RemoveDirectory(string remoteName)
		{
			return false;
		}
	}
}
