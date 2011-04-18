using System;
using System.IO;
using System.Reflection;
using TotalCommander.Plugin.Wfx;
using TotalCommander.Plugin.Wcx;

namespace TotalCommander.Plugin
{
    static class TotalCommanderPluginHolder
    {
        private static ITotalCommanderWfxPlugin wfx;

        private static ITotalCommanderWcxPlugin wcx;


        static TotalCommanderPluginHolder()
        {
            try
            {
                var file = Path.ChangeExtension(Assembly.GetExecutingAssembly().Location, ".dll");
                var assembly = Assembly.LoadFrom(file);
                foreach (var type in assembly.GetExportedTypes())
                {
                    var interfaces = type.GetInterfaces();
                    if (Array.Exists(interfaces, i => i == typeof(ITotalCommanderWfxPlugin)))
                    {
                        wfx = (ITotalCommanderWfxPlugin)Activator.CreateInstance(type);
                        return;
                    }
                    if (Array.Exists(interfaces, i => i == typeof(ITotalCommanderWcxPlugin)))
                    {
                        wcx = (ITotalCommanderWcxPlugin)Activator.CreateInstance(type);
                        return;
                    }
                }
            }
            catch { }
        }


        public static ITotalCommanderWfxPlugin GetWfxPlugin()
        {
            return wfx;
        }

        public static ITotalCommanderWcxPlugin GetWcxPlugin()
        {
            return wcx;
        }
    }
}
