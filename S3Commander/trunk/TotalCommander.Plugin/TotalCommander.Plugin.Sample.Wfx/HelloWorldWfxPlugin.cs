using System;
using System.Collections;
using System.IO;
using TotalCommander.Plugin.Wfx;

namespace TotalCommander.Plugin.Sample.Wfx
{
    [TotalCommanderPlugin("Hello World Plugin")]
    public class HelloWorldWfxPlugin : TotalCommanderWfxPlugin
    {
        public override FindData FindFirst(string path, out IEnumerator enumerator)
        {
            enumerator = null;
            if (path == "\\")
            {
                enumerator = new[] { "Zero", "One", "Two" }.GetEnumerator();
                if (enumerator.MoveNext())
                {
                    return new FindData((string)enumerator.Current, 12345)
                    {
                        LastWriteTime = DateTime.Now,
                    };
                }
            }
            if (path == "\\One")
            {
                return new FindData("File2", 366);
            }
            return FindData.NoMoreFiles;
        }

        public override FindData FindNext(IEnumerator enumerator)
        {
            if (enumerator != null && enumerator.MoveNext())
            {
                return new FindData((string)enumerator.Current, FileAttributes.Directory);
            }
            return FindData.NoMoreFiles;
        }
    }
}
