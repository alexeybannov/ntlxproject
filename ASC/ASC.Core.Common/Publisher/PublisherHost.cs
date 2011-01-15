#region usings

using System;
using System.Collections;
using System.Collections.Generic;
using ASC.Common.Utils;
using ASC.Reflection;

#endregion

namespace ASC.Core.Common.Publisher
{
    public class PublisherHost
        : IPublisherHost
    {
        private static readonly object SyncRoot = new object();

        public PublisherHost()
        {
            Version = new Version(CoreConstResource.Version);
        }

        #region IPublisherHost

        public Version Version { get; internal set; }

        public Hashtable Properties { get; internal set; }

        public void Initialize(IPublisher defaultPublisher, Hashtable properties)
        {
            LogHolder.Log("ASC.Publisher")
                .Debug("initializing publisher host");
            if (!ValidatePublisher(defaultPublisher))
                throw new ArgumentException("Invalid default publisher", "defaultPublisher");
            _defaultPublisher = defaultPublisher;
            Properties = properties ?? new Hashtable();

            CreatePublisherInstance();
            _surrogate = new SurrogatePublisher(this);
        }

        private SurrogatePublisher _surrogate;

        public IPublisher GetPublisher()
        {
            return _surrogate;
        }

        #endregion

        private class SurrogatePublisher : IPublisher
        {
            private readonly PublisherHost _host;

            public SurrogatePublisher(PublisherHost host)
            {
                _host = host;
            }

            #region IPublisher Members

            public Version Version
            {
                get { throw new NotImplementedException(); }
            }

            public Version PublisherVersion
            {
                get { throw new NotImplementedException(); }
            }

            public Hashtable Properties
            {
                get { throw new NotImplementedException(); }
            }

            public void Initialize(Hashtable properties)
            {
                throw new NotImplementedException();
            }

            public List<Article> HandleRequest(RequestContext context, List<Zone> visibleZones)
            {
                if (!_host.ValidatePublisher(_host._publisherInstance))
                    return _host._defaultPublisher.HandleRequest(context, visibleZones);
                try
                {
                    return _host._publisherInstance.HandleRequest(context, visibleZones);
                }
                catch
                {
                    return _host._defaultPublisher.HandleRequest(context, visibleZones);
                }
            }

            #endregion
        }

        #region properties

        public const string PropName_PubInstance = "pubhost.pubinstance";

        internal object Prop_PubInstance
        {
            get { return Properties[PropName_PubInstance]; }
        }

        public const string PropName_PubType = "pubhost.pubtype";

        internal Type Prop_PubType
        {
            get
            {
                var type = Properties[PropName_PubType] as string;
                if (String.IsNullOrEmpty(type)) return null;
                return Type.GetType(type, true);
            }
        }

        #endregion

        internal IPublisher _defaultPublisher;
        internal IPublisher _publisherInstance;

        internal void CreatePublisherInstance()
        {
            LogHolder.Log("ASC.Publisher")
                .Debug("creating publisher instance");
            var ctx = new Context();
            ctx.GetUserManagerClient = () => CoreContext.UserManager;
            ctx.GetConfigurationClient = () => CoreContext.Configuration;
            var inst = Prop_PubInstance as IPublisher;
            if (inst == null)
            {
                Type type = null;
                try
                {
                    type = Prop_PubType;
                }
                catch (Exception exc)
                {
                    LogHolder.Log("ASC.Publisher")
                        .Error("error create publisher type", exc);
                }
                if (type != null)
                {
                    try
                    {
                        inst = TypeInstance.Create(type, ctx) as IPublisher;
                    }
                    catch (Exception exc)
                    {
                        LogHolder.Log("ASC.Publisher")
                            .Error("error create publisher instance", exc);
                    }
                }
                else
                {
                    LogHolder.Log("ASC.Publisher")
                        .Error("publisher type is null");
                }
            }
            if (inst != null)
                inst.Initialize(Properties);
            _publisherInstance = inst;
        }

        internal IPublisher GetPublisherInstance()
        {
            return null;
        }

        internal bool ValidatePublisher(IPublisher publisher)
        {
            if (publisher == null) return false;

            return true;
        }

        #region Loading and testing of new versions

        internal enum SwitchVersionStatus
        {
            Ready,
            Checking,
            ReadyToDownload,
            Downloading,
            ReadyToLoad,
            Loading,
            ReadyToSwitch,
            Switching,
            InvalidVersion,
            Restoring
        }

        internal void ValidatePublisherVersion()
        {
            if (CheckNewVersionMoment())
            {
                if (NewVersionAvailable())
                {
                    BeginDownloadVersion();
                }
            }
            else if (SwitchVersionMoment())
            {
                SwitchVersion();
            }
        }

        internal void SwitchVersion()
        {
            throw new NotImplementedException();
        }

        internal bool SwitchVersionMoment()
        {
            return false;
        }

        internal bool CheckNewVersionMoment()
        {
            return false;
        }

        internal void BeginDownloadVersion()
        {
            throw new NotImplementedException();
        }

        internal void DownloadingCompliteCallback(PublisherHost host)
        {
        }

        internal bool NewVersionAvailable()
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}