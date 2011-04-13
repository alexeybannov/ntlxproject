using System;

namespace ASC.Core
{
    [Serializable]
    public class SubscriptionMethod
    {
        public int Tenant
        {
            get;
            set;
        }

        public string SourceId
        {
            get;
            set;
        }

        public string ActionId
        {
            get;
            set;
        }

        public string RecipientId
        {
            get;
            set;
        }

        public string[] Methods
        {
            get;
            set;
        }
    }
}
