#region usings

using System;
using System.Collections;
using ASC.Notify.Messages;

#endregion

namespace ASC.Notify.Sinks
{
    public abstract class SinkSkeleton
        : ISink
    {
        public SinkSkeleton(IDictionary properties)
        {
            if (properties == null) throw new ArgumentNullException("properties");
            Properties = properties;
        }

        public SinkSkeleton()
        {
            Properties = new Hashtable();
        }

        #region ISink 

        public IDictionary Properties { get; private set; }

        public abstract SendResponse ProcessMessage(INoticeMessage message);

        public virtual void ProcessMessageAsync(INoticeMessage message)
        {
            NextSink.ProcessMessageAsync(message);
        }

        public ISink NextSink { get; set; }

        #endregion
    }
}