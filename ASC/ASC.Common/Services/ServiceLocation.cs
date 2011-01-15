#region usings

using System;
using ASC.Net;

#endregion

namespace ASC.Common.Services
{
    [Serializable]
    public struct ServiceLocation
    {
        public ConnectionHostEntry ConnectionHostEntry;
        public Guid ServiceInstanceID;

        public override string ToString()
        {
            return String.Format("{0}[{1}]", ConnectionHostEntry, ServiceInstanceID);
        }

        public override bool Equals(object obj)
        {
            if (obj is ServiceLocation)
            {
                var l = (ServiceLocation) obj;
                return l.ServiceInstanceID == ServiceInstanceID && l.ConnectionHostEntry.Equals(ConnectionHostEntry);
            }
            return false;
        }

        public override int GetHashCode()
        {
            return (ServiceInstanceID.GetHashCode() ^ ConnectionHostEntry.GetHashCode());
        }
    }
}