#region usings

using System;
using ASC.Notify.Engine;
using ASC.Notify.Recipients;

#endregion

namespace ASC.Notify
{
    public class InitiatorInterceptor
        : SendInterceptorSkeleton
    {
        public InitiatorInterceptor(params IRecipient[] initiators)
            : base(
                "Sys.InitiatorInterceptor",
                InterceptorPlace.GroupSend | InterceptorPlace.DirectSend,
                InterceptorLifetime.Call,
                (request, place) =>
                Array.Exists(initiators ?? new IRecipient[0], rec => request.Recipient.Equals(rec))
                )
        {
        }
    }
}