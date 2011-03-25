using System;
using System.Security.Principal;
using ASC.Common.Security.Authentication;
using ASC.Common.Security.Authorizing;
using Action = ASC.Common.Security.Authorizing.Action;

namespace ASC.Core.Configuration
{
    public sealed class Constants
    {
        public static readonly string NotifyEMailSenderSysName = "email.sender";

        public static readonly string NotifyMessengerSenderSysName = "messanger.sender";


        public static readonly ISysAccount CoreSystem = new SysAccount(new Guid("A37EE56E-3302-4a7b-B67E-DDBEA64CD032"), "asc system");

        public static readonly ISysAccount Guest = new GuestAccount(new Guid("712D9EC3-5D2B-4b13-824F-71F00191DCCA"), "guest");

        public static readonly ISysAccount Demo = new DemoAccount();

        public static readonly ISysAccount Teamlab = new SysAccount(new Guid("FE77D26F-5196-45e0-80B9-E01241F63100"), "Teamlab");

        public static readonly IPrincipal Anonymous = new GenericPrincipal(Guest, new[] { Role.Everyone });

        public static readonly ISysAccount[] SystemAccounts = new[]
        {
            Demo,
            CoreSystem,
            Teamlab
        };
    }
}
