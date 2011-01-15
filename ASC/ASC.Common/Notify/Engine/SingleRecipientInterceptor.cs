#region usings

using System;
using System.Collections.Generic;
using ASC.Notify.Recipients;

#endregion

namespace ASC.Notify.Engine
{
    internal class SingleRecipientInterceptor
        : ISendInterceptor
    {
        private const string prefix = "__singlerecipientinterceptor";
        private readonly List<IRecipient> _sendedTo = new List<IRecipient>(10);

        internal SingleRecipientInterceptor(string name)
        {
            if (String.IsNullOrEmpty(name)) throw new ArgumentException("name");
            Name = name;
        }

        #region ISendInterceptor

        public string Name { get; private set; }

        public bool PreventSend(NotifyRequest request, InterceptorPlace place)
        {
            IRecipient sendTo = request.Recipient;
            if (!_sendedTo.Exists(rec => Equals(rec, sendTo)))
            {
                _sendedTo.Add(sendTo);
                return false;
            }
            else
                return true;
        }

        public InterceptorPlace PreventPlace
        {
            get { return InterceptorPlace.GroupSend | InterceptorPlace.DirectSend; }
        }

        public InterceptorLifetime Lifetime
        {
            get { return InterceptorLifetime.Call; }
        }

        #endregion
    }
}