using TotalCommander.Plugin.Wcx;

namespace TotalCommander.Plugin.WcxSample
{
    public class WcxPlugin : TotalCommanderWcxPlugin
    {
        public override IArchiveUnpacker Unpack(string archiveName, OpenArchiveMode mode)
        {
            return new WcxUnpacker(archiveName, mode);
        }
    }
}
