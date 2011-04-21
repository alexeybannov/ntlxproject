using System;
using System.Collections.Generic;

namespace TotalCommander.Plugin.Wcx
{
    public interface IArchiveUnpacker : IEnumerator<ArchiveHeader>
    {
        void UnpackFile(ArchiveHeader header, string filepath, ArchiveProcess operation);
    }
}
