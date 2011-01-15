#region usings

using System;
using System.Collections.Generic;

#endregion

namespace ASC.Notify.Channels
{
    public class SenderHolder
    {
        private readonly object SyncRoot = new object();
        private readonly Dictionary<object, ISenderChannel> _Channels = new Dictionary<object, ISenderChannel>(1);

        #region public methods

        public void RegisterSender(ISenderChannel sender)
        {
            if (sender == null) throw new ArgumentNullException("sender");
            if (sender.SenderName == null) throw new ArgumentException("sender.SenderName is empty", "sender");
            lock (SyncRoot)
            {
                if (_Channels.ContainsKey(sender.SenderName))
                {
                    if (_Channels[sender.SenderName].Equals(sender)) return;
                    _Channels.Remove(sender.SenderName);
                }
                _Channels.Add(sender.SenderName, sender);
            }
        }

        public void UngeristerSender(ISenderChannel sender)
        {
            if (sender == null) throw new ArgumentNullException("sender");
            if (sender.SenderName == null) throw new ArgumentException("sender.SenderName is empty", "sender");
            lock (SyncRoot)
            {
                if (_Channels.ContainsKey(sender.SenderName))
                    _Channels.Remove(sender.SenderName);
            }
        }

        public ISenderChannel GetSender(string senderName)
        {
            lock (SyncRoot)
            {
                ISenderChannel channel = null;
                _Channels.TryGetValue(senderName, out channel);
                return channel;
            }
        }

        #endregion
    }
}