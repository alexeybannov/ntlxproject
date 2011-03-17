using System.Collections;
using System.Collections.Generic;
using ASC.Common.Utils;
using ASC.Core.Notify;
using ASC.Core.Tenants;
using ASC.Notify.Engine;
using log4net;
using Constants = ASC.Core.Configuration.Constants;
using NotifyContext = ASC.Notify.Context;

namespace ASC.Core
{
    public static class WorkContext
    {
        private static readonly object syncRoot = new object();
        private static readonly Dictionary<string, object> properties = new Dictionary<string, object>(10);
        private static readonly ILog log = LogManager.GetLogger(typeof(WorkContext));

        private static bool notifyStarted;
        private static NotifyContext notifyContext;
        private static readonly NotifySenderDescription[] availableServerNotifySenders = new[]
        {
            new NotifySenderDescription(Constants.NotifyEMailSenderSysName, "by e-mail"),
            new NotifySenderDescription(Constants.NotifyMessengerSenderSysName,"by messenger")
        };


        public static NotifyContext NotifyContext
        {
            get
            {
                NotifyStartUp();
                return notifyContext;
            }
        }

        public static NotifySenderDescription[] AvailableNotifySenders
        {
            get { return new List<NotifySenderDescription>(availableServerNotifySenders).ToArray(); }
        }

        public static NotifySenderDescription[] DefaultClientSenders
        {
            get { return new[] { new NotifySenderDescription(Constants.NotifyEMailSenderSysName, "by e-mail"), }; }
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



        private static void NotifyStartUp()
        {
            if (notifyStarted) return;
            lock (syncRoot)
            {
                if (notifyStarted) return;

                var notifyProperties = new Hashtable();
                notifyProperties[NotifyContext.NotifyDispatcherInstanceKey] = CoreContext.Notify;
                notifyContext = new NotifyContext(notifyProperties);
                foreach (var sender in availableServerNotifySenders)
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
    }
}