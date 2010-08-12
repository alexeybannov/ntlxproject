using System;
using System.Collections.Generic;
using System.IO;
using TotalCommander.Plugin.Wfx;
using TotalCommander.Plugin;

namespace TotalCommander.Plugin.Wfx.FileSystem
{
    public interface IFile
    {
        string Name
        {
            get;
        }

        FileAttributes Attributes
        {
            get;
        }

        long FileSize
        {
            get;
        }

        DateTime? LastWriteTime
        {
            get;
        }

        IEnumerable<IFile> GetChildren();


        ExecuteResult Open(TotalCommanderWindow window, ref string link);

        ExecuteResult Properties(TotalCommanderWindow window, ref string link);
    }
}
