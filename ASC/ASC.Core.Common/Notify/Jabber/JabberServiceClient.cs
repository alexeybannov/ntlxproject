using System;
using System.Collections.Generic;
using System.ServiceModel;

namespace ASC.Core.Notify.Jabber
{
    class JabberServiceClient
    {
        private static readonly TimeSpan timeout = TimeSpan.FromMinutes(2);

        private static DateTime lastErrorTime = default(DateTime);

        private static bool IsServiceProbablyNotAvailable()
        {
            return lastErrorTime != default(DateTime) && lastErrorTime + timeout > DateTime.Now;
        }


        public bool SendMessage(string to, string subject, string text, int tenantId)
        {
            if (IsServiceProbablyNotAvailable()) return false;

            var service = new JabberServiceClientWcf();
            try
            {
                service.SendMessage(to, subject, text, tenantId);
                return true;
            }
            catch (CommunicationException) { lastErrorTime = DateTime.Now; }
            catch (TimeoutException) { lastErrorTime = DateTime.Now; }
            finally
            {
                CloseClient(service);
            }

            return false;
        }

        private class JabberServiceClientWcf : ClientBase<IJabberService>, IJabberService
        {
            public void SendMessage(string to, string subject, string text, int tenantId)
            {
                Channel.SendMessage(to, subject, text, tenantId);
            }

            public IDictionary<string, string> GetClientConfiguration(int tenantId)
            {
                throw new NotImplementedException();
            }

            public int GetNewMessagesCount(string userName, int tenantId)
            {
                throw new NotImplementedException();
            }

            public string GetUserToken(string userName, int tenantId)
            {
                throw new NotImplementedException();
            }

            public void SendCommand(string from, string to, string command, int tenantId)
            {
                throw new NotImplementedException();
            }

            public bool IsUserAvailable(string username, int tenantId)
            {
                throw new NotImplementedException();
            }
        }

        private void CloseClient(JabberServiceClientWcf client)
        {
            try
            {
                client.Close();
            }
            catch (CommunicationException)
            {
                client.Abort();
            }
            catch (TimeoutException)
            {
                client.Abort();
            }
            catch (Exception)
            {
                client.Abort();
                throw;
            }
        }
    }
}
