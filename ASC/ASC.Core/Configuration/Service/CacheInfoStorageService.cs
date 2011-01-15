using ASC.Common.Security;
using ASC.Common.Security.Authentication;
using ASC.Common.Services;
using ASC.Core.Common.Cache;
using ASC.Core.Common.Remoting;
using ASC.Core.Configuration.Service;

[assembly: AssemblyServices(typeof(CacheInfoStorageService))]

namespace ASC.Core.Configuration.Service {
    
	class CacheInfoStorageService : RemotingServiceController, ICacheInfoStorageService {

		private ICacheInfoStorage storage;

		public CacheInfoStorageService()
			: base(Constants.CacheInfoStorageServiceInfo) {

			storage = new CacheInfoStorage();
		}


		#region ICacheInfoStorage Members

        [AuthenticationLevel(SecurityLevel.None)]
		public void RegisterCache(CacheInfo info) {
			storage.RegisterCache(info);
		}

        [AuthenticationLevel(SecurityLevel.None)]
        public CacheInfoStorageResult UpdateCache(CacheVersion version, CacheAction action){
			return storage.UpdateCache(version, action);
		}

        [AuthenticationLevel(SecurityLevel.None)]
        public CacheInfoStorageResult ValidateCache(CacheVersion version){
			return storage.ValidateCache(version);
		}

		#endregion
	}
}