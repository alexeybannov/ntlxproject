#region usings

using System.Collections.Generic;
using ASC.Common.Services;

#endregion

namespace ASC.Core.Notify.Jabber
{
    [Service("{D0593ABF-8B94-4d73-BA3F-AD55ACFAB65E}", ServiceInstancingType.Singleton)]
    public interface IJabberService : IService
    {
        int GetNewMessagesCount(string userName);
        string GetUserToken(string userName);
        IDictionary<string, string> GetClientConfiguration();
        bool IsUserAvailable(string username);
        void SendMessage(string to, string subject, string text);
        void SendCommand(string from, string to, string command);
    }
}