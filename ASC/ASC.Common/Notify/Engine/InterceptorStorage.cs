#region usings

using System;
using System.Collections.Generic;
using System.Runtime.Remoting.Messaging;
using System.Threading;

#endregion

namespace ASC.Notify.Engine
{
    internal class InterceptorStorage
    {
        private static readonly object SSyncRoot = new object();

        private static readonly Dictionary<int, Dictionary<string, ISendInterceptor>> _threads =
            new Dictionary<int, Dictionary<string, ISendInterceptor>>(5);

        internal readonly string CallContext_Prefix = "InterceptorStorage.CALLCONTEXT_KEY." + Guid.NewGuid();
        private readonly object SyncRoot = new object();

        private Dictionary<string, ISendInterceptor> _globalInterceptors;

        #region public methods

        public void Add(ISendInterceptor interceptor)
        {
            if (interceptor == null) throw new ArgumentNullException("interceptor");
            if (String.IsNullOrEmpty(interceptor.Name))
                throw new ArgumentException("empty name property", "interceptor");
            switch (interceptor.Lifetime)
            {
                case InterceptorLifetime.Call:
                    AddInternal(interceptor, CallInterceptors);
                    break;
                case InterceptorLifetime.Thread:
                    AddInternal(interceptor, ThreadInterceptors);
                    break;
                case InterceptorLifetime.Global:
                    AddInternal(interceptor, GlobalInterceptors);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        public ISendInterceptor Get(string name)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentException("empty name", "name");
            ISendInterceptor result = null;
            result = GetInternal(name, CallInterceptors);
            if (result == null)
                result = GetInternal(name, ThreadInterceptors);
            if (result == null)
                result = GetInternal(name, GlobalInterceptors);
            return result;
        }

        public void Remove(string name)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentException("empty name", "name");
            RemoveInternal(name, CallInterceptors);
            RemoveInternal(name, ThreadInterceptors);
            RemoveInternal(name, GlobalInterceptors);
        }

        public void Clear()
        {
            Clear(InterceptorLifetime.Call | InterceptorLifetime.Global | InterceptorLifetime.Thread);
        }

        public void Clear(InterceptorLifetime lifetime)
        {
            lock (SyncRoot)
            {
                if ((lifetime & InterceptorLifetime.Call) == InterceptorLifetime.Call) CallInterceptors.Clear();
                if ((lifetime & InterceptorLifetime.Thread) == InterceptorLifetime.Thread) ThreadInterceptors.Clear();
                if ((lifetime & InterceptorLifetime.Global) == InterceptorLifetime.Global) GlobalInterceptors.Clear();
            }
        }

        public List<ISendInterceptor> GetAll()
        {
            var result = new List<ISendInterceptor>();
            result.AddRange(CallInterceptors.Values);
            result.AddRange(ThreadInterceptors.Values);
            result.AddRange(GlobalInterceptors.Values);
            return result;
        }

        #endregion

        private Dictionary<string, ISendInterceptor> ThreadInterceptors
        {
            get
            {
                Dictionary<string, ISendInterceptor> _threadInterceptors = null;
                lock (SSyncRoot)
                {
                    if (!_threads.TryGetValue(
                        Thread.CurrentThread.ManagedThreadId,
                        out _threadInterceptors
                             ))
                    {
                        _threadInterceptors = new Dictionary<string, ISendInterceptor>(10);
                        _threads[Thread.CurrentThread.ManagedThreadId] = _threadInterceptors;
                    }
                }
                return _threadInterceptors;
            }
        }

        private Dictionary<string, ISendInterceptor> GlobalInterceptors
        {
            get
            {
                if (_globalInterceptors == null)
                    _globalInterceptors = new Dictionary<string, ISendInterceptor>(10);
                return _globalInterceptors;
            }
        }

        private Dictionary<string, ISendInterceptor> CallInterceptors
        {
            get
            {
                var storage = CallContext.GetData(CallContext_Prefix) as Dictionary<string, ISendInterceptor>;
                if (storage == null)
                {
                    storage = new Dictionary<string, ISendInterceptor>(10);
                    CallContext.SetData(CallContext_Prefix, storage);
                }
                return storage;
            }
        }

        private void AddInternal(ISendInterceptor interceptor, Dictionary<string, ISendInterceptor> storage)
        {
            lock (SyncRoot)
            {
                if (storage.ContainsKey(interceptor.Name))
                    storage[interceptor.Name] = interceptor;
                else
                    storage.Add(interceptor.Name, interceptor);
            }
        }

        private ISendInterceptor GetInternal(string name, Dictionary<string, ISendInterceptor> storage)
        {
            ISendInterceptor interceptor = null;
            lock (SyncRoot)
            {
                storage.TryGetValue(name, out interceptor);
            }
            return interceptor;
        }

        private void RemoveInternal(string name, Dictionary<string, ISendInterceptor> storage)
        {
            lock (SyncRoot)
            {
                if (storage.ContainsKey(name))
                    storage.Remove(name);
            }
        }
    }
}