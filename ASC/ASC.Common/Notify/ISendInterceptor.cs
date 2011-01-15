#region usings

using ASC.Notify.Engine;

#endregion

namespace ASC.Notify
{
    public delegate bool PreventSendInterceptor(NotifyRequest request, InterceptorPlace place);

    public interface ISendInterceptor
    {
        string Name { get; }

        InterceptorPlace PreventPlace { get; }

        InterceptorLifetime Lifetime { get; }
        bool PreventSend(NotifyRequest request, InterceptorPlace place);
    }
}