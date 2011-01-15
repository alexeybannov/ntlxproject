#region usings

using System;
using System.Net;
using System.Net.Mail;
using ASC.Notify.Engine;

#endregion

namespace ASC.Notify.Sinks.Smtp
{
    public class SmtpSender
    {
        private readonly SmtpClient smtpClient;

        public SmtpSender(string servername, int port, ICredentialsByHost credentials, bool enableSsl)
        {
            smtpClient = new SmtpClient(servername, port)
                             {
                                 Credentials = credentials,
                                 EnableSsl = enableSsl,
                             };
        }

        public Exception LastError { get; private set; }

        public NoticeSendResult Send(MailMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");
            LastError = null;
            NoticeSendResult result = NoticeSendResult.TryOnceAgain;
            try
            {
                try
                {
                    smtpClient.Send(message);
                }
                catch (Exception exc)
                {
                    LastError = exc;
                    throw;
                }
                result = NoticeSendResult.OK;
            }
            catch (ArgumentNullException)
            {
                result = NoticeSendResult.MessageIncorrect;
            }
            catch (ArgumentOutOfRangeException)
            {
                result = NoticeSendResult.MessageIncorrect;
            }
            catch (ObjectDisposedException)
            {
                result = NoticeSendResult.SendingImpossible;
            }
            catch (InvalidOperationException)
            {
                if (String.IsNullOrEmpty(smtpClient.Host) || smtpClient.Port == 0)
                    result = NoticeSendResult.SendingImpossible;
                else
                    result = NoticeSendResult.TryOnceAgain;
            }
            catch (SmtpFailedRecipientException ex)
            {
                if (
                    ex.StatusCode == SmtpStatusCode.MailboxBusy ||
                    ex.StatusCode == SmtpStatusCode.MailboxUnavailable ||
                    ex.StatusCode == SmtpStatusCode.ExceededStorageAllocation
                    )
                {
                    result = NoticeSendResult.TryOnceAgain;
                }
                else if (
                    ex.StatusCode == SmtpStatusCode.MailboxNameNotAllowed ||
                    ex.StatusCode == SmtpStatusCode.UserNotLocalWillForward ||
                    ex.StatusCode == SmtpStatusCode.UserNotLocalTryAlternatePath
                    )
                {
                    result = NoticeSendResult.MessageIncorrect;
                }
                else if (ex.StatusCode != SmtpStatusCode.Ok)
                    result = NoticeSendResult.TryOnceAgain;
            }
            catch (SmtpException)
            {
                result = NoticeSendResult.SendingImpossible;
            }
            return result;
        }
    }
}