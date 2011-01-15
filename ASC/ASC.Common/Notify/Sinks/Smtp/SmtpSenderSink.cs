#region usings

using System;
using System.Collections;
using System.Net;
using System.Net.Mail;
using System.Text;
using ASC.Common.Utils;
using ASC.Notify.Engine;
using ASC.Notify.Messages;
using ASC.Notify.Patterns;

#endregion

namespace ASC.Notify.Sinks.Smtp
{
    public class SmtpSenderSink
        : SinkSkeleton,
          ISenderSink
    {
        public const string SmtpSenderParam = "smtpsendersink.sender";

        public const string SmtpServerNameParam = "smtpsendersink.servername";

        public const string SmtpPortParam = "smtpsendersink.port";

        public const string SmtpCredentialsParam = "smtpsendersink.credentials";
        public const string SmtpEnableSslParam = "smtpsendersink.enablessl";

        private const string HtmlForm =
            @"<!DOCTYPE html PUBLIC ""-//W3C//DTD HTML 4.01 Transitional//EN""><html><head>  <meta content=""text/html;charset=UTF-8"" http-equiv=""Content-Type""></head><body>{0}</body></html>";

        internal readonly MailAddress _sender;
        internal readonly SmtpSender _smtpSender;

        public SmtpSenderSink(IDictionary properties)
            : base(properties)
        {
            if (properties == null) throw new ArgumentNullException("properties");

            if (!properties.Contains(SmtpSenderParam) || !(properties[SmtpSenderParam] is MailAddress))
            {
                throw new ArgumentException("Properties do not contain required field - smtp sender address",
                                            "properties");
            }
            _sender = (MailAddress) properties[SmtpSenderParam];
            string servername = null;
            if (!properties.Contains(SmtpServerNameParam) ||
                String.IsNullOrEmpty(Convert.ToString(properties[SmtpServerNameParam])))
            {
                throw new ArgumentException("Properties do not contain required field - smtp server name", "properties");
            }
            servername = Convert.ToString(properties[SmtpServerNameParam]);
            int port = 25;
            if (properties.Contains(SmtpPortParam)) port = Convert.ToInt32(properties[SmtpPortParam]);
            ICredentialsByHost credentials = null;
            if (properties.Contains(SmtpCredentialsParam))
            {
                credentials = properties[SmtpCredentialsParam] as ICredentialsByHost;
            }
            bool enableSsl = properties.Contains(SmtpEnableSslParam) ? (bool) properties[SmtpEnableSslParam] : false;

            _smtpSender = new SmtpSender(servername, port, credentials, enableSsl);
        }

        #region ISenderSink Members

        public override SendResponse ProcessMessage(INoticeMessage message)
        {
            if (message == null) throw new ArgumentNullException("message");
            NoticeSendResult result = NoticeSendResult.MessageIncorrect;
            var responce = new SendResponse(message, "", SendResult.Impossible);
            if (message.Recipient.Addresses == null || message.Recipient.Addresses.Length == 0)
                return responce;
            try
            {
                MailMessage emailMessage = BuildMailMessage(message);
                result = _smtpSender.Send(emailMessage);
            }
            catch (Exception exc)
            {
                return new SendResponse(message, "", exc);
            }
            responce.Exception = _smtpSender.LastError;
            switch (result)
            {
                case NoticeSendResult.TryOnceAgain:
                    responce.Result = SendResult.Inprogress;
                    break;
                case NoticeSendResult.MessageIncorrect:
                    responce.Result = SendResult.IncorrectRecipient;
                    break;
                case NoticeSendResult.SendingImpossible:
                    responce.Result = SendResult.Impossible;
                    break;
                default:
                    responce.Result = SendResult.Ok;
                    break;
            }
            return responce;
        }

        #endregion

        internal MailMessage BuildMailMessage(INoticeMessage message)
        {
            var email = new MailMessage();
            email.BodyEncoding = Encoding.UTF8;
            email.SubjectEncoding = Encoding.UTF8;

            email.From = _sender;

            bool nameOne = false;
            foreach (string address in message.Recipient.Addresses)
            {
                MailAddress recipient = !nameOne
                                            ? new MailAddress(
                                                  address,
                                                  message.Recipient.Name
                                                  )
                                            : new MailAddress(address);
                email.To.Add(recipient);
                nameOne = true;
            }

            email.Subject = message.Subject.Trim(' ', '\t', '\n', '\r');
            if (message.ContentType == Pattern.HTMLContentType)
            {
                email.Body = HtmlUtil.GetText(message.Body);

                string html = String.Format(HtmlForm, message.Body);
                AlternateView alternate = AlternateView.CreateAlternateViewFromString(html, Encoding.UTF8, "text/html");

                email.AlternateViews.Add(alternate);
            }
            else
                email.Body = message.Body;
            return email;
        }
    }
}