using ASC.Common.Services;
using ASC.Core.Common.Module;

namespace ASC.Core.Users.Module
{
    class CommonServicesModulePart : ModuleServicesPart
    {
        internal CommonServicesModulePart()
            : base(Constants.ModulePartInfo_Services, Constants.UsersServices)
        {

        }

        protected override IServiceHost CreateServiceHost()
        {
            return new Service.Host(this);
        }
    }
}
