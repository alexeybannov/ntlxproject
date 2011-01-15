#region usings

using System;
using System.Collections.Generic;
using ASC.Notify.Cron;
using ASC.Notify.Model;
using ASC.Notify.Patterns;

#endregion

namespace ASC.Notify.Engine
{
    public class PullNotice
    {
        private readonly List<ITagValue> _Arguments = new List<ITagValue>();

        public PullNotice(INotifySource source, INotifyAction action, string cronString, string objectID,
                          SendNoticeCallback callback, params ITagValue[] args)
        {
            if (source == null) throw new ArgumentNullException("source");
            if (action == null) throw new ArgumentNullException("action");
            if (String.IsNullOrEmpty(cronString)) throw new ArgumentNullException("cronString");
            if (!CronExpression.IsValidExpression(cronString))
                throw new ArgumentException("cronString");
            Source = source;
            Action = action;
            ObjectID = objectID;
            Callback = callback;
            Cron = new CronExpression(cronString);
            _Arguments.AddRange(args);
        }

        public INotifySource Source { get; private set; }

        public INotifyAction Action { get; private set; }

        public CronExpression Cron { get; private set; }

        public string ObjectID { get; private set; }

        public SendNoticeCallback Callback { get; private set; }

        public ITagValue[] Arguments
        {
            get { return _Arguments.ToArray(); }
        }

        public void AddArgument(ITagValue tagValue)
        {
            if (tagValue == null) throw new ArgumentNullException("tagValue");
            if (!_Arguments.Exists(tv => tv.Tag == tagValue.Tag))
                _Arguments.Add(tagValue);
        }
    }
}