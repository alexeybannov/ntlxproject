#region usings

using System;
using System.Security.Principal;
using ASC.Common.Module;
using ASC.Common.Security.Authentication;
using ASC.Common.Security.Authorizing;
using ASC.Common.Services;
using ASC.Common.Utils;
using ASC.Core.Common;
using ASC.Core.Common.Configuration.Resources;
using ASC.Core.Common.Services;
using ASC.Net;
using Action = ASC.Common.Security.Authorizing.Action;

#endregion

namespace ASC.Core.Configuration
{
    public sealed class Constants
    {
        #region services info

        public static readonly ServiceInfoBase ServiceLocatorServiceInfo =
            new CoreServiceInfo(
                typeof (IServiceLocator),
                DescriptionResource.ServiceName_ServiceLocator,
                DescriptionResource.ServiceDescription_ServiceLocator,
                ConstResource.ServiceSysName_ServiceLocator,
                new Version("0.1"),
                new[] {TransportType.Tcp, TransportType.Ipc},
                UriUtil.BuildUri(                    ConstResource.ServiceSysName_ServiceLocator                    )
                );

        public static readonly ServiceInfoBase NotifyServiceInfo =
            new CoreServiceInfo(
                typeof (INotify),
                DescriptionResource.ServiceName_Notify,
                DescriptionResource.ServiceDescription_Notify,
                ConstResource.ServiceSysName_Notify,
                new Version("0.1"),
                new[] {TransportType.Tcp, TransportType.Ipc},
                UriUtil.BuildUri(ConstResource.ServiceSysName_Notify)
                );

        #endregion

        public static readonly ServiceInfoBase[] ConfigurationServices = new[]
                                                                             {
                                                                                 ServiceLocatorServiceInfo,
                                                                                 NotifyServiceInfo,
                                                                             };

        #region Module parts info

        public static readonly ModulePartInfoBase ModulePartInfo_Services = new ModuleServicesPartInfo(
            new Guid(ConstResource.ModulePart_Services_ID),
            DescriptionResource.ModulePart_Services_Name,
            DescriptionResource.ModulePart_Services_Description,
            ConstResource.ModulePart_Services_SysName,
            ModulePartType.Service,
            ConfigurationServices
            );

        public static readonly ModulePartInfoBase ModulePartInfo_HostServices = new ModuleServicesPartInfo(
            new Guid(ConstResource.ModulePart_HostServices_ID),
            DescriptionResource.ModulePart_HostServices_Name,
            DescriptionResource.ModulePart_HostServices_Description,
            ConstResource.ModulePart_HostServices_SysName,
            ModulePartType.Service,
            new[] {Hosting.Constants.ModuleHostServiceInfo}
            );

        public static readonly ModulePartInfoBase ModulePartInfo_Common = new ModulePartInfoBase(
            new Guid(ConstResource.ModulePart_Common_ID),
            DescriptionResource.ModulePart_Common_Name,
            DescriptionResource.ModulePart_Common_Description,
            ConstResource.ModulePart_Common_SysName,
            ModulePartType.Common
            );

        public static readonly ModulePartInfoBase[] ModulePartsInfo = new[]
                                                                          {
                                                                              ModulePartInfo_Common,
                                                                              ModulePartInfo_Services,
                                                                              ModulePartInfo_HostServices
                                                                          };

        #endregion

        #region Module info

        public static readonly IModuleInfo ModuleInfo = new ModuleInfoBase(
            new Guid(ConstResource.Module_ID),
            DescriptionResource.Module_Name,
            DescriptionResource.Module_Description,
            ConstResource.Module_SysName,
            new Version("0.1"),
            ModuleType.CoreInfrastructure,
            ModulePartsInfo
            );

        #endregion

        #region authorization rules module configuration

        public static readonly Action Action_AuthByLoginPwd = new Action(
            new Guid(ConstResource.Action_AuthByLoginPwd_ID),
            DescriptionResource.Action_AuthByLoginPwd_Name,
            DescriptionResource.Action_AuthByLoginPwd_Description,
            ConstResource.Action_AuthByLoginPwd_SysName
            );

        public static readonly Action Action_AuthSettings = new Action(
            new Guid(ConstResource.Action_AuthSettings_ID),
            DescriptionResource.Action_AuthSettings_Name,
            DescriptionResource.Action_AuthSettings_Description,
            ConstResource.Action_AuthSettings_SysName
            );

        public static readonly AuthCategory AuthCategory_Authentication = new AuthCategory(
            new Guid(ConstResource.AuthCategory_Authentication_ID),
            DescriptionResource.AuthCategory_Authentication_Name,
            DescriptionResource.AuthCategory_Authentication_Description,
            ConstResource.AuthCategory_Authentication_SysName,
            new[]
                {
                    Action_AuthByLoginPwd,
                    Action_AuthSettings
                }
            );

        public static readonly Action Action_Configure = new Action(
            new Guid(ConstResource.Action_Configure_ID),
            DescriptionResource.Action_Configure_Name,
            DescriptionResource.Action_Configure_Description,
            ConstResource.Action_Configure_SysName
            );

        public static readonly AuthCategory AuthCategory_Configure = new AuthCategory(
            new Guid(ConstResource.AuthCategory_Configure_ID),
            DescriptionResource.AuthCategory_Configure_Name,
            DescriptionResource.AuthCategory_Configure_Description,
            ConstResource.AuthCategory_Configure_SysName,
            new[]
                {
                    Action_Configure
                }
            );

        public static readonly AuthCategory[] AuthorizingCategories = new[]
                                                                          {
                                                                              AuthCategory_Authentication,
                                                                              AuthCategory_Configure
                                                                          };

        #endregion

        #region key values settings

        internal const string CfgKey_SmtpSettings = "SmtpSettings";

        internal const string CfgKey_ServiceTrustZone = "ServiceTrustZone";

        internal const string CfgKey_DemoAccountEnabled = "DemoAccountEnabled";

        internal const string CfgKey_InstallDate = "InstallDate";

        internal const string CfgKey_InstallID = "InstallID";

        #endregion

        #region Alert

        public static readonly string NotifyEMailSenderSysName = "email.sender";

        public static readonly string NotifyMessengerSenderSysName = "messanger.sender";

        public static readonly string NotifyWebSenderSysName = "web.sender";

        #endregion

        public static readonly ISysAccount CoreSystem = new SysAccount(
            new Guid(ConstResource.SysAccount_CoreSystem_ID),
            DescriptionResource.SysAccount_CoreSystem_Name
            );

        public static readonly ISysAccount Guest = new GuestAccount(
            new Guid(ConstResource.SysAccount_Guest_ID),
            DescriptionResource.SysAccount_Guest_Name
            );

        public static readonly ISysAccount Demo = new DemoAccount();

        public static readonly ISysAccount Teamlab = new SysAccount(
            new Guid("{FE77D26F-5196-45e0-80B9-E01241F63100}"),
            "Teamlab"
            );

        public static readonly ISysAccount[] SystemAccounts = new[]
                                                                  {
                                                                      Demo,
                                                                      CoreSystem,
                                                                      Teamlab
                                                                  };

        public static readonly IPrincipal Anonymus = new GenericPrincipal(Guest, new[] {Role.Everyone});
    }
}