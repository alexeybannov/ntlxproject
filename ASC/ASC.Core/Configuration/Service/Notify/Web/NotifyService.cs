using System;
using System.Web.Services;
using System.Web.Services.Description;
using System.Web.Services.Protocols;

namespace ASC.Core.Configuration.Service.Notify.Web
{
    [WebServiceBinding]
    class NotifyService : SoapHttpClientProtocol
    {
        private const string Namespace = "http://ASC.Web.Studio.WebServices.NotifyService";
        private const string Method = "NotifyHandler";
        private const string Action = Namespace + "/" + Method;

        public NotifyService(string url)
        {
            Url = url;
            UseDefaultCredentials = true;
        }

        [SoapDocumentMethodAttribute(Action,
            RequestNamespace = Namespace,
            ResponseNamespace = Namespace,
            Use = SoapBindingUse.Literal,
            ParameterStyle = SoapParameterStyle.Wrapped)]
        public bool NotifyHandler(Guid userID, string subject, string body, string contentType)
        {
            return (bool)Invoke(Method, new object[] { userID, subject, body, contentType })[0];
        }
    }
}
