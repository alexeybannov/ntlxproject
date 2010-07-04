using TotalCommander.Plugin.Wfx;

namespace TotalCommander.Plugin
{
	static class PluginHolder
	{
		private static ITotalCommanderWFXPlugin wfxPlugin;


		public static ITotalCommanderWFXPlugin GetWfxPlugin()
		{
			if (wfxPlugin == null)
			{

			}
			return wfxPlugin;
		}
	}
}
