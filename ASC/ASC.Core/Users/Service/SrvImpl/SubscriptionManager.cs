using System;
using System.Collections.Generic;
using System.Security.Permissions;
using System.Threading;
using ASC.Common.Services;
using ASC.Core.Common.Remoting;
using ASC.Core.Factories;
using ASC.Core.Users.DAO;
using ASC.Core.Users.Service.SrvImpl;
using log4net;

[assembly: AssemblyServices(typeof(SubscriptionManager))]

namespace ASC.Core.Users.Service.SrvImpl
{
	class SubscriptionManager : RemotingServiceController, ISubscriptionManager
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(SubscriptionManager));
		private readonly IDAOFactory factory;
		private int version = 0;

		internal SubscriptionManager(IDAOFactory daoFactory)
			: base(Constants.SubscriptionManagerServiceInfo)
		{
			if (daoFactory == null) throw new ArgumentNullException("daoFactory");
			factory = daoFactory;
		}

		#region ISubscriptionManager
		[PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
		public void Subscribe(string sourceID, string actionID, string objectID, string recipientID)
		{
			if (sourceID == null) throw new ArgumentNullException("sourceID");
			if (actionID == null) throw new ArgumentNullException("actionID");
			if (recipientID == null) throw new ArgumentNullException("recipientID");

			Interlocked.Increment(ref version);

			try
			{
				using (var dao = GetDAO())
				{
					dao.Subscribe(sourceID, actionID, recipientID, objectID);
				}
			}
			catch (Exception exc)
			{
				throw SMException(DescriptionResource.SME_SaveError, exc);
			}
		}

		[PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
		public void Unsubscribe(string sourceID, string actionID, string objectID, string recipientID)
		{
			if (sourceID == null) throw new ArgumentNullException("sourceID");
			if (actionID == null) throw new ArgumentNullException("actionID");
			if (recipientID == null) throw new ArgumentNullException("recipientID");

			Interlocked.Increment(ref version);

			try
			{
				using (var dao = GetDAO())
				{
					dao.UnSubscribe(sourceID, actionID, recipientID, objectID);
				}
			}
			catch (Exception exc)
			{
				throw SMException(DescriptionResource.SME_SaveError, exc);
			}
		}

		[PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
		public void UnsubscribeAll(string sourceID, string actionID)
		{
			if (sourceID == null) throw new ArgumentNullException("sourceID");
			if (actionID == null) throw new ArgumentNullException("actionID");

			Interlocked.Increment(ref version);

			try
			{
				using (var dao = GetDAO())
				{
					dao.UnSubscribe(sourceID, actionID, null, false);
				}
			}
			catch (Exception exc)
			{
				throw SMException(DescriptionResource.SME_SaveError, exc);
			}
		}

		[PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
		public void UnsubscribeAll(string sourceID, string actionID, string objectID)
		{
			if (sourceID == null) throw new ArgumentNullException("sourceID");
			if (actionID == null) throw new ArgumentNullException("actionID");

			Interlocked.Increment(ref version);

			try
			{
				using (var dao = GetDAO())
				{
					dao.UnSubscribe(sourceID, actionID, objectID, true);
				}
			}
			catch (Exception exc)
			{
				throw SMException(DescriptionResource.SME_SaveError, exc);
			}
		}

		[PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
		public void Unsubscribe(string sourceID, string actionID, string recipientID)
		{
			if (sourceID == null) throw new ArgumentNullException("sourceID");
			if (actionID == null) throw new ArgumentNullException("actionID");
			if (recipientID == null) throw new ArgumentNullException("recipientID");

			Interlocked.Increment(ref version);

			try
			{
				using (var dao = GetDAO())
				{
					dao.UnSubscribe(sourceID, actionID, recipientID);
				}
			}
			catch (Exception exc)
			{
				throw SMException(DescriptionResource.SME_SaveError, exc);
			}
		}

		[PrincipalPermission(SecurityAction.Demand, Authenticated = true)]
		public void UpdateSubscriptionMethod(string sourceID, string actionID, string recipientID, string[] senderNames)
		{
			if (sourceID == null) throw new ArgumentNullException("sourceID");
			if (actionID == null) throw new ArgumentNullException("actionID");
			if (recipientID == null) throw new ArgumentNullException("recipientID");

			Interlocked.Increment(ref version);

			try
			{
				using (var dao = GetDAO())
				{
					dao.UpdateSubscriptionMethod(sourceID, actionID, recipientID, senderNames);
				}
			}
			catch (Exception exc)
			{
				throw SMException(DescriptionResource.SME_SaveError, exc);
			}
		}


		public string[] GetRecipients(string sourceID, string actionID, string objectID)
		{
			if (sourceID == null) throw new ArgumentNullException("sourceID");
			if (actionID == null) throw new ArgumentNullException("actionID");

			List<Subscription> subscriptions = null;
			try
			{
				using (var dao = GetDAO())
				{
					subscriptions = dao.LoadSubscriptions(sourceID, actionID, true, objectID, true, null, false);
				}
			}
			catch (Exception exc)
			{
				throw SMException(DescriptionResource.SME_LoadError, exc);
			}

			List<string> recipients = new List<string>(subscriptions.Count);
			subscriptions.ForEach(sub => { if (!sub.IsUnsubscribed && !recipients.Contains(sub.RecipientID)) recipients.Add(sub.RecipientID); });

			return recipients.ToArray();
		}

		public string[] GetSubscriptionMethod(string sourceID, string actionID, string recipientID)
		{
			if (sourceID == null) throw new ArgumentNullException("sourceID");
			if (actionID == null) throw new ArgumentNullException("actionID");
			if (recipientID == null) throw new ArgumentNullException("recipientID");

			try
			{
				using (var dao = GetDAO())
				{
					return dao.GetSubscriptionMethod(sourceID, actionID, recipientID);
				}
			}
			catch (Exception exc)
			{
				throw SMException(DescriptionResource.SME_LoadError, exc);
			}
		}

		public string[] GetSubscriptions(string sourceID, string actionID, string recipientID)
		{
			if (sourceID == null) throw new ArgumentNullException("sourceID");
			if (actionID == null) throw new ArgumentNullException("actionID");
			if (recipientID == null) throw new ArgumentNullException("recipientID");

			List<Subscription> subscriptions = null;
			try
			{
				using (var dao = GetDAO())
				{
					subscriptions = dao.LoadSubscriptions(sourceID, actionID, true, null, false, recipientID, true);
				}
			}
			catch (Exception exc)
			{
				throw SMException(DescriptionResource.SME_LoadError, exc);
			}

			List<string> objects = new List<string>(subscriptions.Count);
			foreach (var sub in subscriptions)
				if (!sub.IsUnsubscribed && !objects.Contains(sub.ObjectID)) objects.Add(sub.ObjectID);

			return objects.ToArray();
		}

		public bool IsUnsubscribe(string sourceID, string recipientID, string actionID, string objectID)
		{
			if (sourceID == null) throw new ArgumentNullException("sourceID");
			if (actionID == null) throw new ArgumentNullException("actionID");
			if (recipientID == null) throw new ArgumentNullException("recipientID");

			List<Subscription> subscriptions = null;
			try
			{
				using (var dao = GetDAO())
				{
					subscriptions = dao.LoadSubscriptions(sourceID, actionID, true, objectID, true, recipientID, true);
				}
			}
			catch (Exception exc)
			{
				throw SMException(DescriptionResource.SME_LoadError, exc);
			}
			return (subscriptions.Count == 1) && subscriptions[0].IsUnsubscribed;
		}

		public int Version
		{
			get { return version; }
		}

		#endregion

		private ISubscriptionDAO GetDAO()
		{
			return factory.GetSubscriptionDao();
		}

		private SubscriptionManipulationException SMException(string message, Exception exc)
		{
			if (exc != null)
			{
				log.Error("ThrowSMException", exc);
			}
			return new SubscriptionManipulationException(message, exc != null ? exc.ToString() : null);
		}
	}
}