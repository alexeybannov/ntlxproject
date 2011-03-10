using System.Collections.Generic;
using ASC.Common.Services;

namespace ASC.Core.Notify.Jabber
{
    public interface IJabberService
    {
        int GetNewMessagesCount(string userName);

        string GetUserToken(string userName);

        IDictionary<string, string> GetClientConfiguration();

        bool IsUserAvailable(string username);

        void SendMessage(string to, string subject, string text);

        void SendCommand(string from, string to, string command);
    }
}