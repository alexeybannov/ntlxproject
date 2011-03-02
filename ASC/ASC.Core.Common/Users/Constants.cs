using System;
using ASC.Common.Security.Authorizing;
using Action = ASC.Common.Security.Authorizing.Action;
using AuthConst = ASC.Common.Security.Authorizing.Constants;

namespace ASC.Core.Users
{
    public sealed class Constants
    {
        #region system group and category groups

        public static readonly Guid SysGroupCategoryId = new Guid("{7717039D-FBE9-45ad-81C1-68A1AA10CE1F}");

        public static readonly GroupInfo GroupEveryone = new GroupInfo(SysGroupCategoryId)
        {
            ID = AuthConst.Everyone.ID,
            Name = AuthConst.Everyone.Name,
            Description = AuthConst.Everyone.Description
        };

        public static readonly GroupInfo GroupUser = new GroupInfo(SysGroupCategoryId)
        {
            ID = AuthConst.User.ID,
            Name = AuthConst.User.Name,
            Description = AuthConst.User.Description
        };

        public static readonly GroupInfo GroupVisitor = new GroupInfo(SysGroupCategoryId)
        {
            ID = AuthConst.Visitor.ID,
            Name = AuthConst.Visitor.Name,
            Description = AuthConst.Visitor.Description
        };

        public static readonly GroupInfo GroupAdmin = new GroupInfo(SysGroupCategoryId)
        {
            ID = AuthConst.Admin.ID,
            Name = AuthConst.Admin.Name,
            Description = AuthConst.Admin.Description
        };

        public static readonly GroupInfo[] BuildinGroups = new[]
        {
            GroupEveryone,
            GroupUser,
            GroupVisitor,
            GroupAdmin
        };

        #endregion


        #region authorization rules module to work with users

        public static readonly Action Action_EditUser = new Action(
            new Guid("{EF5E6790-F346-4b6e-B662-722BC28CB0DB}"),
            "Edit user information");

        public static readonly Action Action_AddRemoveUser = new Action(
            new Guid("{D5729C6F-726F-457e-995F-DB0AF58EEE69}"),
            "Add/Remove user");

        public static readonly Action Action_EditGroups = new Action(
            new Guid("{1D4FEEAC-0BF3-4aa9-B096-6D6B104B79B5}"),
            "Edit categories and groups");

        public static readonly Action Action_EditAz = new Action(
            new Guid("{1D32DA85-C014-4854-82A1-0F88118BDF94}"),
            "Edit permissions");

        public static readonly AuthCategory AuthCategory_GroupUsers = new AuthCategory(
            new Guid("{84C2CED5-0ABB-4e6d-AAE5-F8A346B85C35}"),
            "Users and groups",
            "Users and groups category actions",
            "Core.GroupUsers",
            new[]
                {
                    Action_EditGroups,
                    Action_AddRemoveUser,
                    Action_EditUser,
                    Action_EditAz,
                }
            );

        public static readonly AuthCategory[] AuthorizingCategories = new[]
        {
            AuthCategory_GroupUsers,
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