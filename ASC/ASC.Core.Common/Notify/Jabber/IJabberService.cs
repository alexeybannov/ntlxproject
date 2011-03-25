using System.Collections.Generic;
using System.ServiceModel;

namespace ASC.Core.Notify.Jabber
{
    [ServiceContract]
    public interface IJabberService
    {
        [OperationContract]
        IDictionary<string, string> GetClientConfiguration(int tenantId);

        [OperationContract]
        bool IsUserAvailable(string username, int tenantId);

        [OperationContract]
        int GetNewMessagesCount(string userName, int tenantId);
        
        [OperationContract]
        string GetUserToken(string userName, int tenantId);
        
        [OperationContract]
        void SendCommand(string from, string to, string command, int tenantId);
        
        [OperationContract]
        void SendMessage(string to, string subject, string text, int tenantId);
    }
}