using System;

namespace TotalCommander.Plugin.Wcx
{
    public interface ITotalCommanderWcxPlugin
    {
        IntPtr OpenArchive(OpenArchiveData archiveData);
    }
}
