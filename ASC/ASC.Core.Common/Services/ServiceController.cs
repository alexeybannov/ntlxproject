#region usings

using System;
using System.Threading;
using ASC.Common.Services;
using ASC.Core.Common.Remoting;
using ASC.Core.Common.Security.Authentication;

#endregion

namespace ASC.Core.Common.Services
{
    public abstract class ServiceController : RemotingServiceController
    {
        private Thread _WorkThread;
        private readonly ManualResetEvent _StopEvent = new ManualResetEvent(false);
        private TimeSpan _StopWait = new TimeSpan(0, 0, 5);

        protected ServiceController(IServiceInfo srvInfo)
            : base(srvInfo)
        {
        }

        protected ServiceController(IServiceInfo srvInfo, Guid serviceInstanceID)
            : base(srvInfo, serviceInstanceID)
        {
        }

        protected override sealed void StartWork()
        {
            BeforeStartWork();
            _DoStartWorkThread(false);
            AfterStartWork();
        }

        protected override sealed void StopWork()
        {
            BeforeStopWork();
            _DoStopWorkThread(StopWait);
            AfterStopWork();
        }

        protected abstract void DoWork();

        protected virtual void BeforeStartWork()
        {
        }

        protected virtual void AfterStartWork()
        {
        }

        protected virtual void BeforeStopWork()
        {
        }

        protected virtual void AfterStopWork()
        {
        }

        protected bool Sleep(TimeSpan period)
        {
            if (_StopEvent == null || _WorkThread == null || !_WorkThread.IsAlive) return false;
            if (period == TimeSpan.MaxValue)
            {
                _StopEvent.WaitOne();
                return true;
            }
            else if (period == TimeSpan.Zero)
            {
                return _StopEvent.WaitOne(0, false);
            }
            else
            {
                return _StopEvent.WaitOne(period, false);
            }
        }

        protected int Wait(TimeSpan period, bool any, params WaitHandle[] handles)
        {
            return any
                       ? WaitHandle.WaitAny(handles, period, false)
                       : WaitHandle.WaitAll(handles, period, false) ? 1 : WaitHandle.WaitTimeout;
        }

        protected bool Working
        {
            get { return _WorkThread != null && _WorkThread.IsAlive; }
        }

        public bool IsBackground { get; set; }
        protected ThreadPriority Priority { get; set; }
        protected TimeSpan StopWait { get; set; }

        private void _DoStopWorkThread(TimeSpan wait)
        {
            if (Working)
            {
                _StopEvent.Set();
                if (!_WorkThread.Join(wait))
                {
                    _WorkThread.Abort();
                    if (!_WorkThread.Join(50))
                    {
                        _WorkThread.Interrupt();
                        if (!_WorkThread.Join(10))
                        {
                            throw new ApplicationException(String.Format("Work thread of service {0} dont't stop!",
                                                                         Info.Name));
                        }
                    }
                }
            }
            _WorkThread = null;
        }

        private void _DoStartWorkThread(bool restart)
        {
            if (Working)
            {
                if (restart) _DoStopWorkThread(StopWait);
                else return;
            }
            _StopEvent.Reset();
            _WorkThread = new Thread(_DoWork);
            _WorkThread.IsBackground = IsBackground;
            _WorkThread.Priority = Priority;
            _WorkThread.Name = String.Format("{0}#work_thread", Info.SysName);
            _WorkThread.Start();
        }

        private void _DoWork()
        {
            Exception exception = null;
            try
            {
                DoWork();
            }
            catch (ThreadAbortException tae)
            {
                exception = tae;
            }
            catch (OutOfMemoryException ome)
            {
                exception = ome;
            }
            catch (Exception ex)
            {
                exception = ex;
                exception.Data["_need_report_"] = true;
            }
            var atsr = new AsyncThreadStopReporter(_WorkThreadStopped);
            atsr.BeginInvoke(exception, null, this);
        }

        private delegate void AsyncThreadStopReporter(Exception exc);

        private void _WorkThreadStopped(Exception exc)
        {
            if (exc != null && Convert.ToBoolean(exc.Data["_need_report_"]))
            {
                var e = new ServiceExceptionEventArgs(Info, exc);
                e.Guilty = GetType();
                FireEvent(e);
            }
            Status = Status & ServiceStatus.Sys_FFFF0000 |
                     ServiceStatus.Stopped |
                     ((exc == null) ? (ServiceStatus.Sys_FF00 & Status) : (ServiceStatus.Error));
        }

        private void _AuthenticateService()
        {
            InternalServiceInfoCache.ServiceInfoEx sinfo = InternalServiceInfoCache.Get(Info.ID);
            if (sinfo == null || !sinfo.IsFixedCoreService)
            {
                SecurityContext.AuthenticateMe(ServiceAccount.CreateFor(this));
            }
        }
    }
}