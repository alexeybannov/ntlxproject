using System;

namespace TotalCommander.Plugin.Wcx
{
    public interface ITotalCommanderWcxPlugin
    {
        void SetDefaultParams(DefaultParam dp);

        BackgroundFlags GetBackgroundFlags();

        PackerCapabilities GetPackerCapabilities();

        bool CanYouHandleThisFile(string fileName);

        
        ArchiveResult OpenArchive(string archiveName, OpenArchiveMode mode, out IntPtr archive);

        ArchiveResult ReadHeader(IntPtr archive, out ArchiveHeader header);

        ArchiveResult ProcessFile(IntPtr archive, ArchiveProcess operation, string filepath, string filename);

        ArchiveResult CloseArchive(IntPtr archive);
    }
}
