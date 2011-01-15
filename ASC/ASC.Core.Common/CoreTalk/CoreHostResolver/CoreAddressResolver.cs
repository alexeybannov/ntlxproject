#region usings

using System;
using System.Collections.Generic;
using ASC.Common.Utils;
using ASC.Net;

#endregion

namespace ASC.Core.Common.CoreTalk
{
    public class CoreAddressResolver : ICoreAddressResolver
    {
        private readonly List<PriorityResolver> resolvers = new List<PriorityResolver>(3);
        private ConnectionHostEntry host;

        public void AddResolver(ICoreAddressResolver resolver, int priority)
        {
            lock (resolvers)
            {
                resolvers.Add(new PriorityResolver(resolver, priority));
                resolvers.Sort((x, y) => { return y.Priority.CompareTo(x.Priority); });
                host = null;
            }
        }

        private class PriorityResolver
        {
            public ICoreAddressResolver Resolver { get; private set; }
            public int Priority { get; private set; }

            public PriorityResolver(ICoreAddressResolver resolver, int priority)
            {
                Resolver = resolver;
                Priority = priority;
            }
        }

        public ConnectionHostEntry GetCoreHostEntry()
        {
            if (host == null)
            {
                lock (resolvers)
                {
                    if (host == null)
                    {
                        foreach (PriorityResolver priorityResolver in resolvers)
                        {
                            ICoreAddressResolver resolver = priorityResolver.Resolver;
                            LogHolder.Log("ASC.Core.Common").DebugFormat("Resolving core address over {0}",
                                                                         resolver.GetType());
                            host = resolver.GetCoreHostEntry();
                            if (host != null)
                            {
                                LogHolder.Log("ASC.Core.Common").InfoFormat("Asc core finded at {0} over {1}",
                                                                            host.Address, resolver.GetType());
                                break;
                            }
                        }
                        if (host == null)
                            throw new ApplicationException("Application couldn't resolve core service address");
                    }
                }
            }
            return host;
        }
    }
}