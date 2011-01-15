#region usings

using System;

#endregion

namespace ASC.Common.Services
{
    public delegate void ServiceUnhandledExceptionEventHandler(object sender, ServiceExceptionEventArgs e);

    [Serializable]
    public class ServiceExceptionEventArgs : ServiceEventsEventArgs
    {
        private readonly Exception _Exception;
        private object _Guilty;
        private bool _Handled;

        public ServiceExceptionEventArgs(IServiceInfo srvInfo, Exception exception)
            :
                base(srvInfo)
        {
            _Exception = exception;
        }

        public Exception Exception
        {
            get { return _Exception; }
        }

        public bool Handled
        {
            get { return _Handled; }
            set { _Handled = value; }
        }

        public object Guilty
        {
            get { return _Guilty; }
            set { _Guilty = value; }
        }
    }
}