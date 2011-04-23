using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using TotalCommander.Plugin.Wcx;

namespace TotalCommander.Plugin.WcxSample
{
    class WcxUnpacker : IArchiveUnpacker
    {
        private readonly IEnumerator<string> files;
        private readonly string archivename;
        private readonly OpenArchiveMode mode;


        public WcxUnpacker(string archivename, OpenArchiveMode mode)
        {
            this.archivename = archivename;
            this.mode = mode;
            files = Directory.GetFiles("d:\\", "*.*").ToList().GetEnumerator();
        }

        public void ExtractCurrentTo(string destfile)
        {

        }

        public void TestCurrent(string destfile)
        {

        }

        public ArchiveHeader Current
        {
            get
            {
                var fi = new FileInfo(files.Current);
                return new ArchiveHeader
                {
                    ArchiveName = archivename,
                    FileAttributes = fi.Attributes,
                    FileName = fi.Name,
                    FileTime = fi.LastWriteTime,
                    UnpackedSize = fi.Length,
                    PackedSize = fi.Length,
                };
            }
        }

        public void Dispose()
        {
            files.Dispose();
        }

        object IEnumerator.Current
        {
            get { return this.Current; }
        }

        public bool MoveNext()
        {
            return files.MoveNext();
        }

        public void Reset()
        {
            files.Reset();
        }

        #region IArchiveUnpacker Members


        public void SetChangeVolume(ChangeVolume changeVolume)
        {
            
        }

        public void SetProgress(Progress progress)
        {
            
        }

        #endregion
    }
}
