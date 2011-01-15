#region usings

using System.Collections;
using ASC.Notify.Channels;

#endregion

namespace ASC.Notify.Sinks
{
    public abstract class SinkProviderSkeleton
        : ISinkProvider
    {
        public SinkProviderSkeleton()
        {
            Properties = new Hashtable();
        }

        #region ISinkProvider 

        public IDictionary Properties { get; private set; }

        public abstract ISink CreateSink(ISenderChannel senderChannel);

        public ISinkProvider Next { get; set; }

        #endregion
    }
}