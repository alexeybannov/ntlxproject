using System;
using TotalCommander.Plugin.Wfx.Internal;

namespace TotalCommander.Plugin.Wfx
{
	public interface ITotalCommanderWfxPlugin
	{
		string Name
		{
			get;
		}

		//void Init(Int32 pluginNumber, ProgressCallback progress, LogCallback log, RequestCallback request)

		bool FindFirst(string path, FindData findData, out object enumerator);

		bool FindNext(object enumerator, FindData findData);

		void FindClose(object enumerator);


		void SetDefaultParams(DefaultParam defaultParam);
	}
}
