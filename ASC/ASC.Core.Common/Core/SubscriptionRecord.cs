using System;

namespace ASC.Core
{
    [Serializable]
    public class SubscriptionRecord
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

        public string ObjectId
        {
            get;
            set;
        }

        public bool Subscribed
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
