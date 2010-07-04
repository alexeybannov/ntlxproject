using System;
using TotalCommander.Plugin.Wfx.Internal;

namespace TotalCommander.Plugin.Wfx
{
	public class Request
	{
		private int pluginNumber;

		private RequestCallback request;

		internal Request(int pluginNumber, RequestCallback request)
		{
			if (request == null) throw new ArgumentNullException("request");

			this.pluginNumber = pluginNumber;
			this.request = request;
		}

		public bool GetRequest(RequestType requestType, string customTitle, string customText, string defaultText)
		{
			//return request(pluginNumber, (int)requestType, customTitle, customText, StringUtil.First(defaultText, 259), 260);
			throw new NotImplementedException();
		}
	}
}
