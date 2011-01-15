using ASC.Common.Module;
using ASC.Common.Security.Authorizing;

namespace ASC.Core.Configuration.Module
{
    class ConfigurationModule : ModuleBase
    {
        public ConfigurationModule()
            : base(
            Constants.ModuleInfo,
            new IModulePart[]{
                new CommonModulePart(),
                new CommonServicesModulePart() 
            })
        {

        }

        public override AuthCategory[] AuthorizingCategories
        {
            get
            {
                return Constants.AuthorizingCategories;
            }
        }
    }
}
