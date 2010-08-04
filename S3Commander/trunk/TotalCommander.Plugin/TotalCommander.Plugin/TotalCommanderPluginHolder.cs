using System;
using System.IO;
using System.Reflection;
using TotalCommander.Plugin.Wfx;

namespace TotalCommander.Plugin
{
	static class TotalCommanderPluginHolder
	{
		private static ITotalCommanderWfxPlugin wfxPlugin;


		public static ITotalCommanderWfxPlugin GetWfxPlugin()
		{
			if (wfxPlugin == null)
			{
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                foreach (var file in Directory.GetFiles(path))
                {
                    try
                    {
                        if (string.Compare(Assembly.GetExecutingAssembly().Location, file, true) == 0) continue;
                        var assembly = Assembly.LoadFrom(file);
                        foreach (var type in assembly.GetExportedTypes())
                        {
                            if (!Attribute.IsDefined(type, typeof(TotalCommanderPluginAttribute))) continue;
                            if (Array.Exists(type.GetInterfaces(), i => i == typeof(ITotalCommanderWfxPlugin)))
                            {
                                wfxPlugin = (ITotalCommanderWfxPlugin)Activator.CreateInstance(type);
                                return wfxPlugin;
                            }
                        }
                    }
                    catch { }
                }
            }
			return wfxPlugin;
		}
	}
}
