using System;

namespace TotalCommander.Plugin.Wfx
{
	public interface ITotalCommanderWFXPlugin
	{
		string Name
		{
			get;
		}

		//void Init(Int32 PluginNr, IntPtr progressProc, IntPtr logProc, IntPtr requestProc)

		bool FindFirst(string path, FindData findData, out object enumerator);

		bool FindNext(object enumerator, FindData findData);

		void FindClose(object enumerator);


		void SetDefaultParams(DefaultParam defaultParam);
	}
}
