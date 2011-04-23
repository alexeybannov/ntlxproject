using System;
using System.Collections.Generic;

namespace TotalCommander.Plugin.Wcx
{
    public interface IArchivePacker
    {
        void PackFiles(string subPath, string sourcePath, string[] files, PackMode mode);

        void DeleteFiles(string[] files);


        void PackInMemory(byte[] inbuffer, out int taken, out byte[] outbuffer);
    }
}
