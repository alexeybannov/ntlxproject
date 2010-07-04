using System;

namespace TotalCommander.Plugin.Wfx
{
	public abstract class TotalCommanderWfxPlugin : ITotalCommanderWfxPlugin
	{
		public abstract string Name
		{
			get;
		}

		//void Init(Int32 PluginNr, IntPtr progressProc, IntPtr logProc, IntPtr requestProc)

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
