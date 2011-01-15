#region usings

using System;

#endregion

namespace ASC.Common.Services
{
    [Serializable]
    public abstract class ServiceEventsEventArgs
        : EventArgs
    {
        #region

        private readonly string _Message = String.Empty;
        private readonly IServiceInfo _ServiceInfo;
        private readonly Guid _ServiceInstanceID = ServiceInfoBase.DefaultInstanceID;

        #endregion

        #region

        protected ServiceEventsEventArgs(IServiceInfo srvInfo)
        {
            if (srvInfo == null)
                throw new ArgumentNullException("srvInfo");
            _ServiceInfo = srvInfo;
        }

        protected ServiceEventsEventArgs(IServiceInfo srvInfo, Guid serviceInstanceID)
        {
            if (srvInfo == null)
                throw new ArgumentNullException("srvInfo");
            _ServiceInfo = srvInfo;
        }

        protected ServiceEventsEventArgs(IServiceInfo srvInfo, string message)
            :
                this(srvInfo)
        {
            _Message = message;
        }

        protected ServiceEventsEventArgs(IServiceInfo srvInfo, Guid serviceInstanceID, string message)
            :
                this(srvInfo, serviceInstanceID)
        {
            _Message = message;
        }

        #endregion

        #region

        public IServiceInfo ServiceInfo
        {
            get { return _ServiceInfo; }
        }

        public Guid ServiceInstanceID
        {
            get { return _ServiceInstanceID; }
        }

        public string Message
        {
            get { return _Message; }
        }

        #endregion
    }
}