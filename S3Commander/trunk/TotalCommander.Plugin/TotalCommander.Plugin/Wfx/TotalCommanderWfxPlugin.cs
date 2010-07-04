using System;

namespace TotalCommander.Plugin.Wfx
{
	public abstract class TotalCommanderWfxPlugin : ITotalCommanderWfxPlugin
	{
		protected Progress Progress
		{
			get;
			private set;
		}

		protected Logger Logger
		{
			get;
			private set;
		}

		protected Request Request
		{
			get;
			private set;
		}


		public void Init(Progress progress, Logger logger, Request request)
		{
			Progress = progress;
			Logger = logger;
			Request = request;
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


		public virtual void SetDefaultParams(DefaultParam defaultParam)
		{

		}
	}
}
