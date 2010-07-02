using System;
using System.IO;

namespace TotalCommander.WFX
{
    public class FindData
    {
        public FileAttributes Attributes
        {
            get;
            set;
        }

        public DateTime CreationTime
        {
            get;
            set;
        }

        public DateTime LastAccessTime
        {
            get;
            set;
        }

        public DateTime LastWriteTime
        {
            get;
            set;
        }

        public long FileSize
        {
            get;
            set;
        }

        public int Reserved0
        {
            get;
            set;
        }

        public int Reserved1
        {
            get;
            set;
        }

        public string FileName
        {
            get;
            set;
        }

        public string AlternateFileName
        {
            get;
            set;
        }
    }
}
