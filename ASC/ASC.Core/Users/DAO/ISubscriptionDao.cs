using System;
using System.Collections.Generic;

namespace ASC.Core.Users.DAO
{
	public interface ISubscriptionDAO : IDisposable
	{
		void Subscribe(string sourceID, string actionID, string recipientID, string objectID);

		void UnSubscribe(string sourceID, string actionID, string recipientID, string objectID);

		void UnSubscribe(string sourceID, string actionID, string recipientID);

		void UnSubscribe(string sourceID, string actionID, string objectID, bool objectFilter);

		List<Subscription> LoadSubscriptions(
			string sourceID,
			string actionID, bool actionFilter,
			string objectID, bool objectFilter,
			string recipientID, bool recipientFilter);


		void UpdateSubscriptionMethod(string sourceID, string actionID, string recipientID, string[] senderNames);

		string[] GetSubscriptionMethod(string sourceID, string actionID, string recipientID);
	}
}
