using System;
using System.Collections.Generic;
using ASC.Common.Module;
using ASC.Common.Services;
using ASC.Core.Common.Remoting;
using ASC.Core.Common.Services;
using ASC.Core.Configuration.Service;
using ASC.Net;
using log4net;

[assembly: AssemblyServices(typeof(ServiceLocator))]

namespace ASC.Core.Configuration.Service
{
	[Locator]
	class ServiceLocator : RemotingServiceController, IServiceLocator
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(ServiceLocator));

		private IDictionary<Guid, IServiceInfo> services = new Dictionary<Guid, IServiceInfo>();
		private IDictionary<Guid, List<ServiceLocation>> serviceAddresses = new Dictionary<Guid, List<ServiceLocation>>();


		internal ServiceLocator()
			: base(Constants.ServiceLocatorServiceInfo)
		{

		}

		#region IService Members

		/// <inheritdoc/>
		public IModulePartInfo ModulePart
		{
			get { return Constants.ModulePartInfo_Services; }
		}

		#endregion

		#region IServiceLocator

		/// <inheritdoc/>
		public ConnectionHostEntry ResolveLocation(Guid serviceID, Guid serviceInstanceID)
		{
			return serviceAddresses.ContainsKey(serviceID) ?
				serviceAddresses[serviceID].Find(l => l.ServiceInstanceID == serviceInstanceID).ConnectionHostEntry :
				null;
		}

		/// <inheritdoc/>
		public ServiceLocation[] InstancedServicesDislocation(Guid serviceID)
		{
			return serviceAddresses.ContainsKey(serviceID) ? serviceAddresses[serviceID].ToArray() : new ServiceLocation[0];
		}

		/// <inheritdoc/>
		public ServiceHostingModel GetServiceHostingModel(Guid serviceID)
		{
			return ServiceHostingModel.Flexible;
		}

		/// <inheritdoc/>
		public void ServiceStartedUp(IServiceInfo srvInfo, Guid serviceInstanceID, ConnectionHostEntry hostEntry)
		{
			log.InfoFormat("service \"{0}[{2}]\" started up at {1}", srvInfo, hostEntry.Address, serviceInstanceID);

			services[srvInfo.ID] = srvInfo;

			if (!serviceAddresses.ContainsKey(srvInfo.ID)) serviceAddresses.Add(srvInfo.ID, new List<ServiceLocation>());
			var location = new ServiceLocation() { ConnectionHostEntry = hostEntry, ServiceInstanceID = serviceInstanceID };
			if (!serviceAddresses[srvInfo.ID].Contains(location)) serviceAddresses[srvInfo.ID].Add(location);
		}

		/// <inheritdoc/>
		public IServiceInfo GetServiceInfo(Guid serviceId)
		{
			if (!services.ContainsKey(serviceId)) throw new ServiceNotFoundException(serviceId.ToString());
			return services[serviceId];
		}

		/// <inheritdoc/>
		public void ServiceShutDowned(Guid serviceID, Guid serviceInstanceID)
		{
			var srvInfo = GetServiceInfo(serviceID);
			log.InfoFormat("service \"{0}[{1}]\" shuted down", srvInfo, serviceInstanceID);

			serviceAddresses[serviceID].RemoveAll(l => l.ServiceInstanceID == serviceInstanceID);
			if (serviceAddresses[serviceID].Count == 0)
			{
				serviceAddresses.Remove(serviceID);
				services.Remove(serviceID);
			}
		}

		#endregion
    }
}