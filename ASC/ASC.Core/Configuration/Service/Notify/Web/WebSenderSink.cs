using System;
using ASC.Notify.Messages;
using ASC.Notify.Sinks;
using log4net;

namespace ASC.Core.Configuration.Service.Notify.Web
{
    class WebSenderSink : SinkSkeleton, ISenderSink
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(WebSenderSink));

        private static readonly string relativeUrl = "/WebServices/NotifyService.asmx";

        private readonly string url;

        private NotifyService webService;


        public WebSenderSink(string webSiteUrl)
        {
            url = webSiteUrl + relativeUrl;
            log.DebugFormat("{0}->{1}", Constants.NotifyWebSenderSysName, url);
        }

        public override SendResponse ProcessMessage(INoticeMessage message)
        {
            SendResponse responce = null;
            try
            {
                var userId = new Guid(message.Recipient.ID);
                var webResult = WebService.NotifyHandler(userId, message.Subject, message.Body, message.ContentType);
                responce = new SendResponse(message, Constants.NotifyWebSenderSysName, webResult ? SendResult.Ok : SendResult.Impossible);
            }
            catch (Exception ex)
            {
                log.Debug("error send to " + url, ex);
                responce = new SendResponse(message, Constants.NotifyWebSenderSysName, ex);
            }
            return responce;
        }


        private NotifyService WebService
        {
            get
            {
                if (webService == null) webService = new NotifyService(url);
                return webService;
            }
        }
    }
}
