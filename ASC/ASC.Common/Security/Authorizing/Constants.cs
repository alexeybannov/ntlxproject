#region usings

using System;

#endregion

namespace ASC.Common.Security.Authorizing
{
    public sealed class Constants
    {
        public static readonly Role Everyone = new Role(
            new Guid(CommonDescriptionResource.SysRole_Everyone_ID),
            CommonDescriptionResource.SysRole_Everyone_Name,
            CommonDescriptionResource.SysRole_Everyone_Description
            );

        public static readonly Role Service = new Role(
            new Guid(CommonDescriptionResource.SysRole_Service_ID),
            CommonDescriptionResource.SysRole_Service_Name,
            CommonDescriptionResource.SysRole_Service_Description
            );

        public static readonly Role User = new Role(
            new Guid(CommonDescriptionResource.SysRole_User_ID),
            CommonDescriptionResource.SysRole_User_Name,
            CommonDescriptionResource.SysRole_User_Description
            );

        public static readonly Role PowerUser = new Role(
            new Guid(CommonDescriptionResource.SysRole_PowerUser_ID),
            CommonDescriptionResource.SysRole_PowerUser_Name,
            CommonDescriptionResource.SysRole_PowerUser_Description
            );

        public static readonly Role Member = new Role(
            new Guid(CommonDescriptionResource.SysRole_Member_ID),
            CommonDescriptionResource.SysRole_Member_Name,
            CommonDescriptionResource.SysRole_Member_Description
            );

        public static readonly Role Owner = new Role(
            new Guid(CommonDescriptionResource.SysRole_Owner_ID),
            CommonDescriptionResource.SysRole_Owner_Name,
            CommonDescriptionResource.SysRole_Owner_Description
            );

        public static readonly Role Self = new Role(
            new Guid(CommonDescriptionResource.SysRole_Self_ID),
            CommonDescriptionResource.SysRole_Self_Name,
            CommonDescriptionResource.SysRole_Self_Description
            );

        public static readonly Role ResourceAdmin = new Role(
            new Guid(CommonDescriptionResource.SysRole_ResourceAdmin_ID),
            CommonDescriptionResource.SysRole_ResourceAdmin_Name,
            CommonDescriptionResource.SysRole_ResourceAdmin_Description
            );

        public static readonly Role Admin = new Role(
            new Guid(CommonDescriptionResource.SysRole_Admin_ID),
            CommonDescriptionResource.SysRole_Admin_Name,
            CommonDescriptionResource.SysRole_Admin_Description
            );

        public static readonly Role Demo = new Role(
            new Guid(CommonDescriptionResource.SysRole_Demo_ID),
            CommonDescriptionResource.SysRole_Demo_Name,
            CommonDescriptionResource.SysRole_Demo_Description
            );

        public static readonly Role Visitor = new Role(
            new Guid(CommonDescriptionResource.SysRole_Visitor_ID),
            CommonDescriptionResource.SysRole_Visitor_Name,
            CommonDescriptionResource.SysRole_Visitor_Description
            );

        public static Role[] SystemRoles = new[]
                                               {
                                                   Everyone,
                                                   Demo,
                                                   Owner,
                                                   Self,
                                                   Visitor,
                                                   User,
                                                   Member,
                                                   PowerUser,
                                                   ResourceAdmin
                                               };
    }
}