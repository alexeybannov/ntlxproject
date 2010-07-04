using System;
using TotalCommander.Plugin.Wfx.Internal;

namespace TotalCommander.Plugin.Wfx
{
	public interface ITotalCommanderWfxPlugin
	{
		void Init(Progress progress, Logger logger, Request request);

		bool FindFirst(string path, FindData findData, out object enumerator);

		bool FindNext(object enumerator, FindData findData);

		void FindClose(object enumerator);


		void SetDefaultParams(DefaultParam defaultParam);
	}
}
