#region usings

using System;

#endregion

namespace ASC.Common.Services
{
    public delegate void HostStatusChangeEventEventHandler(object sender, HostStatusChangeEventArgs e);

    [Serializable]
    public class HostStatusChangeEventArgs
    {
        private readonly string _HostName;
        private readonly HostStatus _NewStatus = HostStatus.Unknown;
        private readonly HostStatus _PreviousStatus = HostStatus.Unknown;

        public HostStatusChangeEventArgs(string hostName, HostStatus previousStatus, HostStatus newStatus)
        {
            _PreviousStatus = previousStatus;
            _NewStatus = newStatus;
            _HostName = hostName;
        }

        public string HostName
        {
            get { return _HostName; }
        }

        public HostStatus PreviousStatus
        {
            get { return _PreviousStatus; }
        }

        public HostStatus NewStatus
        {
            get { return _NewStatus; }
        }
    }
}