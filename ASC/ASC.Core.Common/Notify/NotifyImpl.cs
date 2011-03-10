using System;
using System.Collections;
using System.Net;
using System.Net.Mail;
using ASC.Common.Utils;
using ASC.Core.Configuration;
using ASC.Core.Notify.Jabber;
using ASC.Notify.Messages;
using ASC.Notify.Sinks.Smtp;
using log4net;
using NotifyContext = ASC.Notify.Context;

namespace ASC.Core.Notify
{
    class NotifyImpl : INotify
    {
        private readonly NotifyContext notifyContext = new NotifyContext();

        private readonly bool logOnly;

        private static readonly ILog log = LogManager.GetLogger("ASC.Notify");

        private static readonly ILog logMessages = LogManager.GetLogger("ASC.Notify.Messages");


        public NotifyImpl()
        {
            logOnly = bool.TrueString.Equals(WorkContext.GetProperty("Notify.LogOnly") as string, StringComparison.InvariantCultureIgnoreCase);
            log.DebugFormat("LogOnly: {0}", logOnly);

            log.Debug(@"Add jabber sender sink");
            notifyContext.NotifyService.RegisterSender(Constants.NotifyMessengerSenderSysName, new JabberSenderSink());
        }


        public void DispatchNotice(INoticeMessage message, string senderName)
        {
            using (DebugUtil.Watch(string.Format("Send by {0} to {1}", senderName, message.Recipient)))
            {
                if (!logOnly)
                {
                    UpdateSmtpSender();
                    notifyContext.DispatchEngine.Dispatch(message, senderName);
                }
                LogMessage(message, senderName);
            }
        }

        public SendResponse DispatchNoticeSync(INoticeMessage message, string senderName)
        {
            var response = new SendResponse(message, senderName, SendResult.Ok);
            if (!logOnly)
            {
                UpdateSmtpSender();
                response = notifyContext.DispatchEngine.Dispatch(message, senderName);
            }
            LogMessage(message, senderName);
            return response;
        }

        private void UpdateSmtpSender()
        {
            notifyContext.NotifyService.UnregisterSender(Constants.NotifyEMailSenderSysName);

            var prop = new Hashtable();
            var settings = CoreContext.Configuration.SmtpSettings;
            try
            {
                prop[SmtpSenderSink.SmtpServerNameParam] = settings.Host;
                prop[SmtpSenderSink.SmtpSenderParam] = new MailAddress(settings.SenderAddress, settings.SenderDisplayName);
                if (settings.Port.HasValue) prop[SmtpSenderSink.SmtpPortParam] = settings.Port.Value;

                if (!string.IsNullOrEmpty(settings.CredentialsUserName))
                {
                    var credential = new NetworkCredential(settings.CredentialsUserName, settings.CredentialsUserPassword, settings.CredentialsDomain);
                    prop[SmtpSenderSink.SmtpCredentialsParam] = credential;
                }
                prop[SmtpSenderSink.SmtpEnableSslParam] = settings.EnableSSL;

                notifyContext.NotifyService.RegisterSender(Constants.NotifyEMailSenderSysName, new SmtpSenderSink(prop));
            }
            catch (Exception exc)
            {
                log.Error("Incorrent smtp settings", exc);
            }
        }

        private void LogMessage(INoticeMessage message, string senderName)
        {
            try
            {
                if (logMessages.IsInfoEnabled)
                {
                    logMessages.InfoFormat("[{5}]->[{1}] by [{6}] to [{2}] at {0}\r\n\r\n[{3}]\r\n{4}\r\n{7}",
                        DateTime.Now,
                        message.Recipient.Name,
                        0 < message.Recipient.Addresses.Length ? message.Recipient.Addresses[0] : string.Empty,
                        message.Subject,
                        (message.Body ?? string.Empty).Replace(Environment.NewLine, Environment.NewLine + @"   "),
                        message.Action,
                        senderName,
                        new string('-', 80));
                }
            }
            catch { }
        }
    }
}
