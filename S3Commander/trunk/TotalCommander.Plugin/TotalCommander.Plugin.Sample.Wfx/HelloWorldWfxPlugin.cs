using System;
using System.Collections;
using TotalCommander.Plugin.Wfx;

namespace TotalCommander.Plugin.Sample.Wfx
{
    [TotalCommanderPlugin("Hello World Plugin")]
    public class HelloWorldWfxPlugin : TotalCommanderWfxPlugin
    {
        public override bool FindFirst(string path, FindData findData, out object enumerator)
        {
            var en = new[] { "Zero", "One", "Two" }.GetEnumerator();
            enumerator = en;

            if (en.MoveNext())
            {
                findData.FileName = (string)en.Current;
				findData.FileSize = 12345;
				findData.LastWriteTime = DateTime.Now;
                return true;
            }
            return false;
        }

        public override bool FindNext(object enumerator, FindData findData)
        {
            var en = (IEnumerator)enumerator;
            if (en.MoveNext())
            {
                findData.FileName = (string)en.Current;
                return true;
            }
			return false;
        }
    }
}
