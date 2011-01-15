#region usings

using System;
using System.Collections;
using ASC.Notify.Channels;
using ASC.Notify.Engine;
using ASC.Notify.Model;

#endregion

namespace ASC.Notify
{
    public sealed class Context
    {
        #region Constants

        public const string SYS_RECIPIENT_ID = "_#" + _SYS_RECIPIENT_ID + "#_";
        internal const string _SYS_RECIPIENT_ID = "SYS_RECIPIENT_ID";
        public const string SYS_RECIPIENT_NAME = "_#" + _SYS_RECIPIENT_NAME + "#_";
        internal const string _SYS_RECIPIENT_NAME = "SYS_RECIPIENT_NAME";
        public const string SYS_RECIPIENT_ADDRESS = "_#" + _SYS_RECIPIENT_ADDRESS + "#_";
        internal const string _SYS_RECIPIENT_ADDRESS = "SYS_RECIPIENT_ADDRESS";

        #endregion

        public const string NotifyDispatcherInstanceKey = "context.dipatcher";
        public const string SkeepErrorsKey = "context.skeep_errors";

        internal readonly object SyncRoot = new object();

        public Context(IDictionary properties)
        {
            if (properties == null) throw new ArgumentNullException("properties");
            Properties = properties;
            if (Properties.Contains(NotifyDispatcherInstanceKey))
            {
                _NotifyDispatcher = (INotifyDispatcher) Properties[NotifyDispatcherInstanceKey];
                Properties.Remove(NotifyDispatcherInstanceKey);
            }
        }

        public Context()
            : this(new Hashtable())
        {
        }

        public IDictionary Properties { get; private set; }

        #region basic classes for the system

        private DispatchEngine _DispatchEngine;
        private INotifyDispatcher _NotifyDispatcher;
        private NotifyEngine _NotifyEngine;
        private INotifyService _NotifyService;
        private SenderHolder _SenderHolder;

        public SenderHolder SenderHolder
        {
            get
            {
                if (_SenderHolder == null)
                    lock (SyncRoot)
                    {
                        if (_SenderHolder == null)
                            _SenderHolder = new SenderHolder();
                    }
                return _SenderHolder;
            }
            internal set { _SenderHolder = value; }
        }

        public INotifyService NotifyService
        {
            get
            {
                if (_NotifyService == null)
                    lock (SyncRoot)
                    {
                        if (_NotifyService == null)
                            _NotifyService = new NotifyServiceImpl(this);
                    }
                return _NotifyService;
            }
            internal set { _NotifyService = value; }
        }

        public INotifyDispatcher NotifyDispatcher
        {
            get
            {
                if (_NotifyDispatcher == null)
                    lock (SyncRoot)
                    {
                        if (_NotifyDispatcher == null)
                            _NotifyDispatcher = new LocalNotifyDispatcher(this);
                    }
                return _NotifyDispatcher;
            }
            internal set { _NotifyDispatcher = value; }
        }

        public NotifyEngine NotifyEngine
        {
            get
            {
                if (_NotifyEngine == null)
                    lock (SyncRoot)
                    {
                        if (_NotifyEngine == null)
                            _NotifyEngine = new NotifyEngine(this);
                    }
                return _NotifyEngine;
            }
            internal set { _NotifyEngine = value; }
        }

        public DispatchEngine DispatchEngine
        {
            get
            {
                if (_DispatchEngine == null)
                    lock (SyncRoot)
                    {
                        if (_DispatchEngine == null)
                            _DispatchEngine = new DispatchEngine(this);
                    }
                return _DispatchEngine;
            }
            internal set { _DispatchEngine = value; }
        }

        #endregion

        #region access to the internal settings

        #endregion

        #region methods to intercept

        internal void Invoke_NotifyClientRegistration(INotifyClient client)
        {
            if (NotifyClientRegistration != null)
                NotifyClientRegistration(this, client);
        }

        public event Action<Context, INotifyClient> NotifyClientRegistration;

        #endregion
    }
}