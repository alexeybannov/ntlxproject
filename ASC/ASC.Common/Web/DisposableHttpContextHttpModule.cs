#region usings

using System;
using System.Web;

#endregion

namespace ASC.Common.Web
{
    public class DisposableHttpContextHttpModule : IHttpModule
    {
        #region IHttpModule

        public void Dispose()
        {
        }

        public void Init(HttpApplication context)
        {
            context.EndRequest += Application_EndRequest;
        }

        #endregion

        private void Application_EndRequest(Object source, EventArgs e)
        {
            var application = (HttpApplication) source;
            new DisposableHttpContext(application.Context).Dispose();
        }
    }
}