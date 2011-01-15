#region usings

using System;
using ASC.Common.Services;

#endregion

namespace ASC.Core.Common.Remoting
{
    public abstract class RemotingServiceController : ServiceControllerBase
    {
        protected RemotingServiceController(IServiceInfo srvInfo)
            : base(srvInfo, WorkContext.ServicePublisher.ServiceInstanceIDResolver.Resolve(srvInfo))
        {
        }

        protected RemotingServiceController(IServiceInfo srvInfo, Guid serviceInstanceID)
            : base(srvInfo, serviceInstanceID)
        {
        }

        protected override sealed void StartInternal()
        {
            try
            {
                Publish();
                StartWork();
            }
            catch
            {
                throw;
            }
            finally
            {
            }
        }

        protected override sealed void StopInternal()
        {
            StopWork();
            UnPublish();
        }

        private void Publish()
        {
            if (!(this is IService)) throw new ServiceDefinitionException(Info.Name);
            WorkContext.ServicePublisher.Publish(this);
        }

        private void UnPublish()
        {
            if (!(this is IService)) throw new ServiceDefinitionException(Info.Name);
            WorkContext.ServicePublisher.UnPublish(this);
        }

        protected virtual void StartWork()
        {
        }

        protected virtual void StopWork()
        {
        }
    }
}