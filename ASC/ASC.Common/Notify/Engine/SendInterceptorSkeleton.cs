#region usings

using System;

#endregion

namespace ASC.Notify.Engine
{
    public class SendInterceptorSkeleton
        : ISendInterceptor
    {
        private readonly PreventSendInterceptor _preventMethod;

        public SendInterceptorSkeleton(string name, InterceptorPlace preventPlace, InterceptorLifetime lifetime,
                                       PreventSendInterceptor sendInterceptor)
        {
            if (String.IsNullOrEmpty("name")) throw new ArgumentNullException("name");
            if (String.IsNullOrEmpty("sendInterceptor")) throw new ArgumentNullException("sendInterceptor");
            _preventMethod = sendInterceptor;
            Name = name;
            PreventPlace = preventPlace;
            Lifetime = lifetime;
        }

        #region ISendInterceptor

        public string Name { get; internal set; }

        public bool PreventSend(NotifyRequest request, InterceptorPlace place)
        {
            bool prevented = false;
            try
            {
                prevented = _preventMethod(request, place);
            }
            catch
            {
            }
            return prevented;
        }

        public InterceptorPlace PreventPlace { get; internal set; }

        public InterceptorLifetime Lifetime { get; internal set; }

        #endregion
    }
}