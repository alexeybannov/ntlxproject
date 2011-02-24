#region usings

using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using ASC.Common.Services;
using ASC.Common.Services.Factories;
using ASC.Common.Utils;
using ASC.Core.Common;
using ASC.Core.Common.CoreTalk;
using ASC.Core.Common.Remoting;
using ASC.Core.Common.Services;
using ASC.Core.Common.Services.Factories;
using ASC.Core.Notify;
using ASC.Core.Tenants;
using ASC.Notify.Engine;
using ASC.Runtime.Remoting;
using log4net;
using Constants = ASC.Core.Configuration.Constants;
using NotifyContext = ASC.Notify.Context;

#endregion

namespace ASC.Core
{
    public static class WorkContext
    {
        private static readonly object syncRoot = new object();
        private static readonly Dictionary<string, object> properties = new Dictionary<string, object>(10);
        private static readonly CoreAddressResolver coreAddressResolver = new CoreAddressResolver();
        private static IDictionary<Type, IServiceFactory> serviceFactories;
        private static readonly RemotingSubsystem remotingSubsystem = new RemotingSubsystem();
        private static readonly IServicePublisher servicePublisher = new ServicePublisher();
        private static readonly IServiceActivator serviceActivator = new ServiceActivator();

        private static readonly ILog log = LogManager.GetLogger(typeof(WorkContext));

        public static ICoreAddressResolver CoreAddressResolver
        {
            get { return coreAddressResolver; }
        }

        public static IServicePublisher ServicePublisher
        {
            get { return servicePublisher; }
        }

        public static IServiceActivator ServiceActivator
        {
            get { return serviceActivator; }
        }

        internal static RemotingSubsystem RemotingSubsystem
        {
            get { return remotingSubsystem; }
        }

        static WorkContext()
        {
            log.Info("Working Context starting up...");
            AddCoreAddressResolver(new ConfigAddressResolver(), 1);
            log.Debug("Resolving core configuration address...");
            CoreAddressResolver.GetCoreHostEntry();
            FindAndRegisterServices();
        }

        public static void AddCoreAddressResolver(ICoreAddressResolver resolver, int priority)
        {
            coreAddressResolver.AddResolver(resolver, priority);
        }

        public static object GetProperty(string propName)
        {
            if (propName == null) return null;
            lock (properties)
            {
                return properties.ContainsKey(propName) ? properties[propName] : null;
            }
        }

        public static void SetProperty(string propName, object value)
        {
            if (propName == null) return;
            lock (properties)
            {
                properties[propName] = value;
            }
        }

        private static void FindAndRegisterServices()
        {
            Array.ForEach(AppDomain.CurrentDomain.GetAssemblies(), a => FindAndRegisterServicesInAssembly(a));
            AppDomain.CurrentDomain.AssemblyLoad += DomainAssemblyLoaded;
        }

        private static void FindAndRegisterServicesInAssembly(Assembly assembly)
        {
            foreach (AssemblyServicesAttribute attribute in GetAssemblyAttributes<AssemblyServicesAttribute>(assembly))
            {
                Array.ForEach(attribute.ServicesTypes, type => ServicePublisher.ServiceLocatorRegistry.Register(type));
            }
            foreach (
                AssemblyServiceHostAttribute attribute in GetAssemblyAttributes<AssemblyServiceHostAttribute>(assembly))
            {
                ServicePublisher.HostLocatorRegistry.Register(attribute.ServiceModulePartID, attribute.HostType);
                break;
            }
        }

        private static void DomainAssemblyLoaded(object sender, AssemblyLoadEventArgs args)
        {
            FindAndRegisterServicesInAssembly(args.LoadedAssembly);
        }

        private static T[] GetAssemblyAttributes<T>(Assembly assembly) where T : Attribute
        {
            return (T[])assembly.GetCustomAttributes(typeof(T), true);
        }

        private static NotifyContext notifyContext;

        public static NotifyContext NotifyContext
        {
            get
            {
                NotifyStartUp();
                return notifyContext;
            }
        }

        private static readonly NotifySenderDescription[] availableServerNotifySenders = new[]
        {
            new NotifySenderDescription(Constants.NotifyEMailSenderSysName, "by e-mail"),
            new NotifySenderDescription(Constants.NotifyMessengerSenderSysName,"by messenger")
        };

        public static NotifySenderDescription[] AvailableNotifySenders
        {
            get
            {
                return new List<NotifySenderDescription>(availableServerNotifySenders).ToArray();
            }
        }

        public static NotifySenderDescription[] DefaultClientSenders
        {
            get
            {
                return new[] { new NotifySenderDescription(Constants.NotifyEMailSenderSysName, "by e-mail"), };
            }
        }

        private static bool notifyStarted;

        private static void NotifyStartUp()
        {
            if (notifyStarted) return;
            lock (syncRoot)
            {
                if (notifyStarted) return;
                var notifyProperties = new Hashtable();
                notifyProperties.Add(NotifyContext.NotifyDispatcherInstanceKey, CoreContext.Notify);
                notifyContext = new NotifyContext(notifyProperties);
                foreach (NotifySenderDescription sender in availableServerNotifySenders)
                {
                    notifyContext.NotifyService.RegisterClientSender(sender.ID);
                }

                notifyContext.NotifyEngine.BeforeTransferRequest += NotifyEngine_BeforeTransferRequest;
                notifyContext.NotifyEngine.AfterTransferRequest += NotifyEngine_AfterTransferRequest;
                notifyStarted = true;
            }
        }

        private static void NotifyEngine_OnAsyncSendThreadStart(NotifyContext context)
        {
            if (!SecurityContext.IsAuthenticated)
            {
                SecurityContext.AuthenticateMe(Constants.CoreSystem);
                LogHolder.Log("ASC.Notify").DebugFormat("Authenticated.");
            }
        }

        private static void NotifyEngine_BeforeTransferRequest(NotifyEngine sender, NotifyRequest request)
        {
            request.Properties.Add("Tenant", CoreContext.TenantManager.GetCurrentTenant(false));
        }

        private static void NotifyEngine_AfterTransferRequest(NotifyEngine sender, NotifyRequest request)
        {
            var tenant = (request.Properties.Contains("Tenant") ? request.Properties["Tenant"] : null) as Tenant;
            if (tenant != null) CoreContext.TenantManager.SetCurrentTenant(tenant);
        }

        #region ServiceFactory

        public static T GetService<T>() where T : IService
        {
            if (serviceFactories == null)
            {
                lock (syncRoot)
                {
                    if (serviceFactories == null)
                    {
                        serviceFactories = new Dictionary<Type, IServiceFactory>(10);
                        var section =
                            ConfigurationManager.GetSection("ServiceFactory") as ServiceFactoryConfigurationSection;
                        if (section != null)
                        {
                            foreach (FactoryElement f in section.Factories)
                            {
                                serviceFactories.Add(
                                    !string.IsNullOrEmpty(f.Service) ? Type.GetType(f.Service, true) : typeof(IService),
                                    (IServiceFactory)Activator.CreateInstance(Type.GetType(f.FactoryType, true)));
                            }
                        }
                        else
                        {
                            log.Debug("ServiceFactory section not found. ServiceFactory initialized default values.");
                            serviceFactories.Add(typeof(IService), new RemotingCoreServiceFactory());
                        }
                    }
                }
            }
            Type type = typeof(T);
            IServiceFactory factory = null;
            if (!serviceFactories.TryGetValue(type, out factory))
            {
                if (!serviceFactories.TryGetValue(typeof(IService), out factory))
                {
                    throw new Exception("Service factory not found for " + type);
                }
            }
            return (T)factory.GetService(type);
        }

        #endregion
    }
}