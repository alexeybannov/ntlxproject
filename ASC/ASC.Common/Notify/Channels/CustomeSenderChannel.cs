#region usings

using ASC.Notify.Engine;
using ASC.Notify.Sinks;

#endregion

namespace ASC.Notify.Channels
{
    public class CustomeSenderChannel
        : SenderChannelSkeleton
    {
        private readonly INotifyDispatcher _cutomeDispatcher;

        public CustomeSenderChannel(Context context, INotifyDispatcher cutomeDispatcher, string name,
                                    ISenderSink senderSink)
            : base(context, name, senderSink)
        {
            _cutomeDispatcher = cutomeDispatcher;
        }

        public CustomeSenderChannel(Context context, string name, ISenderSink senderSink)
            : this(context, null, name, senderSink)
        {
        }

        protected override ISenderSink DoSenderSink()
        {
            return null;
        }

        protected override INotifyDispatcher GetDispatcher()
        {
            if (_cutomeDispatcher == null)
                return Context.NotifyDispatcher;
            else
                return _cutomeDispatcher;
        }
    }
}