using System;

namespace ASC.Core.Users.DAO
{
	[Serializable]
	public class Subscription
	{
		internal int ID
		{
			get;
			set;
		}

		public Subscription()
		{
			IsUnsubscribed = false;
		}

		public string SourceID;

		public string ActionID;

		public string RecipientID;

		public string ObjectID;
		
		public bool IsUnsubscribed;
	}
}
