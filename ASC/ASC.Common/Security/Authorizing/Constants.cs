#region usings

using System;

#endregion

namespace ASC.Common.Security.Authorizing
{
    public sealed class Constants
    {
        public static readonly Role Admin = new Role(new Guid("{CD84E66B-B803-40fc-99F9-B2969A54A1DE}"), "Admin", "System administrator");

        public static readonly Role Everyone = new Role(new Guid("{C5CC67D1-C3E8-43c0-A3AD-3928AE3E5B5E}"), "Everyone", "Everyone");


        public static readonly Role User = new Role(new Guid("{ABEF62DB-11A8-4673-9D32-EF1D8AF19DC0}"), "User", "User, employee of company");

        public static readonly Role Visitor = new Role(new Guid("{AAAAF67E-37B9-4e8e-9612-130C1A1CDA64}"), "Visitor", "Visitor");

        public static readonly Role Demo = new Role(new Guid("{64A18D36-7D1B-4509-9616-4C3DBD043DE2}"), "Demo", "Demo user account");


        public static readonly Role Member = new Role(new Guid("{BA74CA02-873F-43dc-8470-8620C156BC67}"), "Member", "Member of something");

        public static readonly Role Owner = new Role(new Guid("{BBA32183-A14D-48ed-9D39-C6B4D8925FBF}"), "Owner", "Object owner");

        public static readonly Role Self = new Role(new Guid("{5D5B7260-F7F7-49f1-A1C9-95FBB6A12604}"), "Self", "Self");


        public static Role[] SystemRoles = new[]
                                               {
                                                   Everyone,
                                                   Demo,
                                                   Owner,
                                                   Self,
                                                   Visitor,
                                                   User,
                                                   Member,
                                               };
    }
}