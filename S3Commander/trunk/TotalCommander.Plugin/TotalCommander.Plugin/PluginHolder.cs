using System;
using System.IO;
using System.Reflection;
using TotalCommander.Plugin.Wfx;

namespace TotalCommander.Plugin
{
	static class PluginHolder
	{
		private static string wfxPluginName;

		private static ITotalCommanderWfxPlugin wfxPlugin;


		public static string GetWfxPluginName()
		{
			if (string.IsNullOrEmpty(wfxPluginName))
			{
				LoadWfxPlugin();
			}
			return wfxPluginName;
		}

		public static ITotalCommanderWfxPlugin GetWfxPlugin()
		{
			if (wfxPlugin == null)
			{
				LoadWfxPlugin();
			}
			return wfxPlugin;
		}


		private static void LoadWfxPlugin()
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
						var attributes = type.GetCustomAttributes(typeof(TotalCommanderPluginAttribute), false);
						if (attributes.Length == 0) continue;

						var attribute = (TotalCommanderPluginAttribute)attributes[0];
						wfxPluginName = attribute.Name;
						if (Array.Exists(type.GetInterfaces(), i => i == typeof(ITotalCommanderWfxPlugin)))
						{
							wfxPlugin = (ITotalCommanderWfxPlugin)Activator.CreateInstance(type);
							return;
						}
					}
				}
				catch { }
			}
		}
	}
}
