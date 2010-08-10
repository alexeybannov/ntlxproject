using System;
using System.Collections.Generic;
using System.IO;
using TotalCommander.Plugin.Wfx;
using TotalCommander.Plugin;

namespace AmazonS3Commander
{
    interface IFile
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


        ExecuteResult Open(TotalCommanderWindow window);
    }
}
