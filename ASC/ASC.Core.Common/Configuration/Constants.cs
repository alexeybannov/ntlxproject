using System;
using System.Security.Principal;
using ASC.Common.Security.Authentication;
using ASC.Common.Security.Authorizing;
using Action = ASC.Common.Security.Authorizing.Action;

namespace ASC.Core.Configuration
{
    public sealed class Constants
    {
        #region authorization rules module configuration

        public static readonly Action Action_AuthByLoginPwd = new Action(
            new Guid("{9D75A568-52AA-49d8-AD43-473756CD8903}"),
            "Login/Password authentication");

        public static readonly Action Action_AuthSettings = new Action(
            new Guid("{1FBB76D4-BA0F-4bed-85A1-5B77C5500E57}"),
            "Authentication settings");

        public static readonly Action Action_Configure = new Action(
            new Guid("{9DCA1F0B-3A61-4cae-9234-FBDD06B8B042}"),
            "System configure");

        public static readonly AuthCategory AuthCategory_Authentication = new AuthCategory(
            new Guid("{B3B961FB-6B00-4ebf-9B9B-DE8182B4B014}"),
            "Authentication",
            "Authentication",
            "Core.Authentication",
            new[] { 
                Action_AuthByLoginPwd, 
                Action_AuthSettings,
                Action_Configure});

        public static readonly AuthCategory[] AuthorizingCategories = new[]
        {
            AuthCategory_Authentication,
        };

        #endregion


        #region Alert

        public static readonly string NotifyEMailSenderSysName = "email.sender";

        public static readonly string NotifyMessengerSenderSysName = "messanger.sender";

        #endregion


        public static readonly ISysAccount CoreSystem = new SysAccount(
            new Guid("{A37EE56E-3302-4a7b-B67E-DDBEA64CD032}"),
            "asc system");

        public static readonly ISysAccount Guest = new GuestAccount(
            new Guid("{712D9EC3-5D2B-4b13-824F-71F00191DCCA}"),
            "guest");

        public static readonly ISysAccount Demo = new DemoAccount();
                
        public static readonly ISysAccount Teamlab = new SysAccount(
            new Guid("{FE77D26F-5196-45e0-80B9-E01241F63100}"),
            "Teamlab");

        public static readonly IPrincipal Anonymus = new GenericPrincipal(Guest, new[] { Role.Everyone });

        public static readonly ISysAccount[] SystemAccounts = new[]
        {
            Demo,
            CoreSystem,
            Teamlab
        };
    }
}
