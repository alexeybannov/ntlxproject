using ASC.Common.Services;
using ASC.Core.Common.Module;

namespace ASC.Core.Configuration.Module
{
    class CommonServicesModulePart : ModuleServicesPart
    {
        internal CommonServicesModulePart()
            : base(Constants.ModulePartInfo_Services, Constants.ConfigurationServices)
        {

        }

        protected override IServiceHost CreateServiceHost()
        {
            return new ASC.Core.Configuration.Service.Host(this);
        }
    }
}
