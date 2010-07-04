using TotalCommander.Plugin.Wfx;
using System.IO;
using System.Reflection;
using System;

namespace TotalCommander.Plugin
{
	static class PluginHolder
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
						var assembly = Assembly.LoadFrom(file);
						foreach (var type in assembly.GetExportedTypes())
						{
							var attributes = type.GetCustomAttributes(typeof(TotalCommanderPluginAttribute), false);
							if (attributes.Length == 0) continue;

							var attribute = attributes[0];
							if (type.IsAssignableFrom(typeof(ITotalCommanderWfxPlugin)))
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
