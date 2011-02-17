using System;
using System.Collections.Generic;
using System.Globalization;

namespace ASC.Core.Tenants
{
    public class TenantRegistrationInfo
    {
        public string Name
        {
            get;
            set;
        }

        public string Address
        {
            get;
            set;
        }

        public List<string> TrustedDomains
        {
            get;
            private set;
        }

        public CultureInfo Culture
        {
            get;
            set;
        }

        public TimeZoneInfo TimeZoneInfo
        {
            get;
            set;
        }

        public string FirstName
        {
            get;
            set;
        }

        public string LastName
        {
            get;
            set;
        }

        public string Email
        {
            get;
            set;
        }

        public string Password
        {
            get;
            set;
        }


        public TenantRegistrationInfo()
        {
            TrustedDomains = new List<string>();
            Culture = CultureInfo.CurrentCulture;
            TimeZoneInfo = TimeZoneInfo.Local;
        }
    }
}
