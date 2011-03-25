using System;
using ASC.Notify.Messages;
using ASC.Notify.Sinks;

namespace ASC.Core.Notify.Jabber
{
    class JabberSenderSink : SinkSkeleton, ISenderSink
    {
        private static readonly string sender = ASC.Core.Configuration.Constants.NotifyMessengerSenderSysName;

        private JabberServiceClient service = new JabberServiceClient();


        public override SendResponse ProcessMessage(INoticeMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");

            var result = SendResult.Ok;
            try
            {
                var user = CoreContext.UserManager.GetUsers(new Guid(message.Recipient.ID));
                if (ASC.Core.Users.Constants.LostUser.Equals(user.ID))
                {
                    result = SendResult.IncorrectRecipient;
                }
                else
                {
                    var success = service.SendMessage(
                        user.UserName,
                        message.Subject,
                        message.Body,
                        CoreContext.TenantManager.GetCurrentTenant().TenantId);

                    if (!success) result = SendResult.Impossible;
                }
            }
            catch (Exception ex)
            {
                return new SendResponse(message, sender, ex);
            }

            return new SendResponse(message, sender, result);
        }
    }
}
