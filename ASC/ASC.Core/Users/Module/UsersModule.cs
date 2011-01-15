using ASC.Common.Module;

namespace ASC.Core.Users.Module
{
    class UsersModule        : ModuleBase
    {
        public UsersModule()
            : base(
                    Constants.ModuleInfo,
                    new IModulePart[]{
                            new CommonModulePart(),
                            new CommonServicesModulePart()
                        }
                )
        {

        }

        public override ASC.Common.Security.Authorizing.AuthCategory[] AuthorizingCategories
        {
            get
            {
                return ASC.Core.Users.Constants.AuthorizingCategories;
            }
        }
    }
}
