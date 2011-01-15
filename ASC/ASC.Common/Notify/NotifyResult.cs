#region usings

using System.Collections.Generic;
using System.Text;
using ASC.Notify.Messages;

#endregion

namespace ASC.Notify
{
    public class NotifyResult
    {
        internal NotifyResult()
        {
            Result = 0;
            Responses = new List<SendResponse>();
        }

        internal NotifyResult(SendResult result, List<SendResponse> responses)
        {
            Result = result;
            Responses = responses ?? new List<SendResponse>();
        }

        #region

        public SendResult Result { get; internal set; }

        public List<SendResponse> Responses { get; set; }

        #endregion

        internal void Merge(NotifyResult result)
        {
            Result |= result.Result;
            Responses.AddRange(result.Responses);
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendFormat("SendResult: {0} whith {1} sub-results", Result, Responses.Count);
            foreach (SendResponse responce in Responses)
            {
                string recipient = "<recipient:nomessage>";
                string error = "";
                if (responce.NoticeMessage != null)
                {
                    if (responce.NoticeMessage.Recipient != null)
                        if (responce.NoticeMessage.Recipient.Addresses.Length > 0)
                            recipient = responce.NoticeMessage.Recipient.Addresses[0];
                        else
                            recipient = "<no-address>";
                    else
                        recipient = "<null-address>";
                }
                if (responce.Exception != null) error = responce.Exception.Message;
                sb.AppendLine();
                sb.AppendFormat("   {3}->{0}({1})={2} {4}", recipient, responce.SenderName, responce.Result,
                                responce.NotifyAction.ID, error);
            }
            return sb.ToString();
        }
    }
}