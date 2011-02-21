#region usings

using System;
using ASC.Common.Module;
using ASC.Common.Security.Authorizing;
using ASC.Common.Services;
using ASC.Common.Utils;
using ASC.Core.Common.Services;
using ASC.Core.Common.Users.Resources;
using ASC.Net;
using Action = ASC.Common.Security.Authorizing.Action;

#endregion

namespace ASC.Core.Users
{
    public sealed class Constants
    {
        #region system group and category groups

        public static readonly Guid SysGroupCategoryId = new Guid("{7717039D-FBE9-45ad-81C1-68A1AA10CE1F}");

        public static readonly GroupInfo GroupEveryone = new GroupInfo(SysGroupCategoryId)
        {
            ID = ASC.Common.Security.Authorizing.Constants.Everyone.ID,
            Name = ASC.Common.Security.Authorizing.Constants.Everyone.Name,
            Description = ASC.Common.Security.Authorizing.Constants.Everyone.Description
        };

        public static readonly GroupInfo GroupUser = new GroupInfo(SysGroupCategoryId)
        {
            ID = ASC.Common.Security.Authorizing.Constants.User.ID,
            Name = ASC.Common.Security.Authorizing.Constants.User.Name,
            Description = ASC.Common.Security.Authorizing.Constants.User.Description
        };

        public static readonly GroupInfo GroupVisitor = new GroupInfo(SysGroupCategoryId)
        {
            ID = ASC.Common.Security.Authorizing.Constants.Visitor.ID,
            Name = ASC.Common.Security.Authorizing.Constants.Visitor.Name,
            Description = ASC.Common.Security.Authorizing.Constants.Visitor.Description
        };

        public static readonly GroupInfo GroupAdmin = new GroupInfo(SysGroupCategoryId)
        {
            ID = ASC.Common.Security.Authorizing.Constants.Admin.ID,
            Name = ASC.Common.Security.Authorizing.Constants.Admin.Name,
            Description = ASC.Common.Security.Authorizing.Constants.Admin.Description
        };

        public static readonly GroupInfo[] BuildinGroups = new[]
        {
            GroupEveryone, GroupUser, GroupVisitor, GroupAdmin
        };

        #endregion

        #region Services

        public static readonly ServiceInfoBase AuthorizationManagerServiceInfo =
            new CoreServiceInfo(
                typeof(IAuthorizationManager),
                DescriptionResource.ServiceName_AuthorizationManager,
                DescriptionResource.ServiceDescription_AuthorizationManager,
                ConstResource.ServiceSysName_AuthorizationManager,
                new Version("0.1"),
                new[] { TransportType.Tcp, TransportType.Ipc },
                UriUtil.BuildUri(ConstResource.ServiceSysName_AuthorizationManager)
                );

        public static readonly ServiceInfoBase SubscriptionManagerServiceInfo =
            new CoreServiceInfo(
                typeof(ISubscriptionManager),
                DescriptionResource.ServiceName_SubscriptionManager,
                DescriptionResource.ServiceDescription_SubscriptionManager,
                ConstResource.ServiceSysName_SubscriptionManager,
                new Version("0.1"),
                new[] { TransportType.Tcp, TransportType.Ipc },
                UriUtil.BuildUri(ConstResource.ServiceSysName_SubscriptionManager)
                );

        #endregion

        public static readonly ServiceInfoBase[] UsersServices = new[]
                                                                     {
                                                                         AuthorizationManagerServiceInfo,
                                                                         SubscriptionManagerServiceInfo
                                                                     };

        #region the modules

        public static readonly ModulePartInfoBase ModulePartInfo_Common = new ModulePartInfoBase(
            new Guid(ConstResource.ModulePart_Common_ID),
            DescriptionResource.ModulePart_Common_Name,
            DescriptionResource.ModulePart_Common_Description,
            ConstResource.ModulePart_Common_SysName,
            ModulePartType.Common
            );

        public static readonly ModulePartInfoBase ModulePartInfo_Services = new ModuleServicesPartInfo(
            new Guid(ConstResource.ModulePart_Services_ID),
            DescriptionResource.ModulePart_Services_Name,
            DescriptionResource.ModulePart_Services_Description,
            ConstResource.ModulePart_Services_SysName,
            ModulePartType.Service,
            UsersServices
            );

        #endregion

        public static readonly ModulePartInfoBase[] ModulePartsInfo = new[]
                                                                          {
                                                                              ModulePartInfo_Common,
                                                                              ModulePartInfo_Services
                                                                          };

        #region Module info

        public static readonly IModuleInfo ModuleInfo = new ModuleInfoBase(
            new Guid(ConstResource.Module_ID),
            DescriptionResource.Module_Name,
            DescriptionResource.Module_Description,
            ConstResource.Module_SysName,
            CoreConstants.CoreVersion,
            ModuleType.CoreInfrastructure,
            ModulePartsInfo
            );

        #endregion

        public static readonly string CacheIdUsers = "{EA97B7DF-ED78-4645-8BFF-57AF0FB7CB82}";

        public static readonly string CacheIdGroups = "{0315C868-E753-43db-9222-8CDBA460F927}";

        public static readonly string CacheIdCategories = "{CFCEC037-BC5C-48c8-A1E5-E3A5868DE645}";

        public static readonly string CacheIdGroupUserRef = "{807EE0EB-DD39-46aa-9A58-E4B0CD9FEB4A}";

        public static readonly string CacheIdAzAce = "{9995AC64-8CC1-4f65-B05B-D488A71739DE}";

        public static readonly string CacheIdAzObjectInfo = "{51C98904-8185-4225-A66C-827DC1FCA5EA}";

        public static readonly string CacheIdTenants = "{EC3D1538-5A79-43D3-9482-103B5AA2507E}";

        #region authorization rules module to work with users

        public static readonly Action Action_EditUser = new Action(
            new Guid(ConstResource.Action_EditUser_ID),
            DescriptionResource.Action_EditUser_Name,
            DescriptionResource.Action_EditUser_Description,
            ConstResource.Action_EditUser_SysName
            );

        public static readonly Action Action_AddRemoveUser = new Action(
            new Guid("{D5729C6F-726F-457e-995F-DB0AF58EEE69}"),
            DescriptionResource.Action_AddRemoveUser_Name,
            DescriptionResource.Action_AddRemoveUser_Description,
            ConstResource.Action_AddRemoveUser_SysName
            );

        public static readonly Action Action_EditGroups = new Action(
            new Guid(ConstResource.Action_EditGroups_ID),
            DescriptionResource.Action_EditGroups_Name,
            DescriptionResource.Action_EditGroups_Description,
            ConstResource.Action_EditGroups_SysName
            );

        public static readonly AuthCategory AuthCategory_GroupUsers = new AuthCategory(
            new Guid(ConstResource.AuthCategory_GroupUsers_ID),
            DescriptionResource.AuthCategory_GroupUsers_Name,
            DescriptionResource.AuthCategory_GroupUsers_Description,
            ConstResource.AuthCategory_GroupUsers_SysName,
            new[]
                {
                    Action_EditGroups,
                    Action_AddRemoveUser,
                    Action_EditUser,
                }
            );

        public static readonly Action Action_EditAz = new Action(
            new Guid(ConstResource.Action_EditAz_ID),
            DescriptionResource.Action_EditAz_Name,
            DescriptionResource.Action_EditAz_Description,
            ConstResource.Action_EditAz_SysName
            );

        public static readonly AuthCategory AuthCategory_Az = new AuthCategory(
            new Guid(ConstResource.AuthCategory_Az_ID),
            DescriptionResource.AuthCategory_Az_Name,
            DescriptionResource.AuthCategory_Az_Description,
            ConstResource.AuthCategory_Az_SysName,
            new[]
                {
                    Action_EditAz
                }
            );

        public static readonly AuthCategory[] AuthorizingCategories = new[]
                                                                          {
                                                                              AuthCategory_GroupUsers,
                                                                              AuthCategory_Az
                                                                          };

        #endregion

        #region "lost groups and users"

        public static readonly UserInfo LostUser = new UserInfo
                                                       {
                                                           ID = new Guid("{4A515A15-D4D6-4b8e-828E-E0586F18F3A3}"),
                                                           FirstName = "Unknown",
                                                           LastName = "Unknown"
                                                       };

        public static readonly GroupInfo LostGroupInfo = new GroupInfo
                                                             {
                                                                 ID = new Guid("{74B9CBD1-2412-4e79-9F36-7163583E9D3A}"),
                                                                 Name = "Unknown"
                                                             };

        #endregion
    }
}