using System;
using ASC.Core.Notify.Jabber;
using ASC.Notify.Messages;
using ASC.Notify.Sinks;

namespace ASC.Core.Notify.Jabber
{
    class JabberSenderSink : SinkSkeleton, ISenderSink
    {
        private readonly static string senderName = ASC.Core.Configuration.Constants.NotifyMessengerSenderSysName;


        public override SendResponse ProcessMessage(INoticeMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");

            try
            {
                var user = CoreContext.UserManager.GetUsers(new Guid(message.Recipient.ID));
                if (ASC.Core.Users.Constants.LostUser.Equals(user.ID))
                {
                    return new SendResponse(message, senderName, SendResult.IncorrectRecipient);
                }

                if (!Available) return new SendResponse(message, senderName, SendResult.Impossible);
                JabberService.SendMessage(user.UserName, message.Subject, message.Body);
            }
            catch (Exception ex)
            {
                return new SendResponse(message, senderName, ex);
            }

            return new SendResponse(message, senderName, SendResult.Ok);
        }


        private object syncRoot = new object();

        private DateTime lastTime;

        private IJabberService jabberService;

        private IJabberService JabberService
        {
            get
            {
                if (jabberService == null && TimeSpan.FromMinutes(1) < (DateTime.Now - lastTime).Duration())
                {
                    lock (syncRoot)
                    {
                        if (jabberService == null && TimeSpan.FromMinutes(1) < (DateTime.Now - lastTime).Duration())
                        {
                            lastTime = DateTime.Now;
                            try
                            {
                                jabberService = WorkContext.ServiceActivator.Activate<IJabberService>();
                            }
                            catch { }
                        }
                    }
                }
                return jabberService;
            }
        }

        public bool Available
        {
            get
            {
                try
                {
                    return JabberService != null && JabberService.Info != null;
                }
                catch
                {
                    jabberService = null;
                    lastTime = DateTime.Now;
                    return false;
                }
            }
        }
    }
}
