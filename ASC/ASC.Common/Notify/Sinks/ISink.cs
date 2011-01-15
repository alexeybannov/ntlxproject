#region usings

using System.Collections;
using ASC.Notify.Messages;

#endregion

namespace ASC.Notify.Sinks
{
    public interface ISink
    {
        IDictionary Properties { get; }
        ISink NextSink { get; set; }

        SendResponse ProcessMessage(INoticeMessage message);

        void ProcessMessageAsync(INoticeMessage message);
    }
}