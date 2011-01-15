#region usings

using System.Collections;
using ASC.Notify.Channels;

#endregion

namespace ASC.Notify.Sinks
{
    public interface ISinkProvider
    {
        IDictionary Properties { get; }

        ISinkProvider Next { get; set; }
        ISink CreateSink(ISenderChannel senderChannel);
    }
}