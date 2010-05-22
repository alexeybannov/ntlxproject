using System;
using log4net;

namespace Ntlx.Server.Utils
{
	static class EventRaiser
	{
		private static readonly ILog log = LogManager.GetLogger(typeof(EventRaiser));


		public static void SoftRaiseEvent(MulticastDelegate @event, object sender, EventArgs e)
		{
			try
			{
				@event.DynamicInvoke(sender, e);
			}
			catch (Exception ex)
			{
				log.Warn(ex);
			}
		}
	}
}
