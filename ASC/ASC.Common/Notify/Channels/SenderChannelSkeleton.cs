#region usings

using System;
using System.Collections;
using ASC.Notify.Engine;
using ASC.Notify.Messages;
using ASC.Notify.Model;
using ASC.Notify.Sinks;

#endregion

namespace ASC.Notify.Channels
{
    public abstract class SenderChannelSkeleton
        : ISenderChannel
    {
        protected readonly Context Context;
        private readonly IDictionary _Properties = new Hashtable();
        private DispatchSink _DispatcherSink;
        private ISink _FirstSink;
        private ISenderSink _SenderSink;

        private bool isInited;

        public SenderChannelSkeleton(Context context, string senderName, INoticeFormatterSink decorateSink,
                                     ISenderSink senderSink)
        {
            if (senderName == null) throw new ArgumentNullException("senderName");
            if (context == null) throw new ArgumentNullException("context");
            Context = context;
            SenderName = senderName;
            _FirstSink = decorateSink;
            _SenderSink = senderSink;
        }

        public SenderChannelSkeleton(Context context, string senderName, ISenderSink senderSink)
            : this(context, senderName, null, senderSink)
        {
        }

        #region ISenderChannel

        public string SenderName { get; private set; }

        public IDictionary Properties
        {
            get { return _Properties; }
        }

        public SendResponse Send(INoticeMessage message, INotifySource source)
        {
            if (message == null) throw new ArgumentNullException("message");
            if (source == null) throw new ArgumentNullException("source");
            Init();
            SendResponse response = _FirstSink.ProcessMessage(message);
            return response;
        }

        public void SendAsync(INoticeMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");
            Init();
            _FirstSink.ProcessMessageAsync(message);
        }

        public SendResponse DirectSend(INoticeMessage message)
        {
            return _SenderSink.ProcessMessage(message);
        }

        #endregion

        #region

        protected virtual ISink DoDefaultFirstSink()
        {
            return null;
        }

        protected abstract ISenderSink DoSenderSink();
        protected abstract INotifyDispatcher GetDispatcher();

        #endregion

        protected virtual void Init()
        {
            if (isInited) return;

            if (_SenderSink == null)
            {
                _SenderSink = DoSenderSink();
                if (_SenderSink == null)
                    throw new ApplicationException(String.Format("channel with tag {0} not created sender sink",
                                                                 SenderName));
            }
            _DispatcherSink = new DispatchSink(SenderName, GetDispatcher());

            _FirstSink = SinkChainHelper.AddSink(_FirstSink, _DispatcherSink);
            isInited = true;
        }
    }
}