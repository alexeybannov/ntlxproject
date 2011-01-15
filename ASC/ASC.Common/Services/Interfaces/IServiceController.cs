namespace ASC.Common.Services
{
    public interface IServiceController : IController
    {
        IServiceInfo Info { get; }

        ServiceStatus Status { get; }

        #region

        event ServiceUnhandledExceptionEventHandler UnhandledException;

        event ServiceStatusChangeEventEventHandler StatusChange;

        #endregion
    }
}