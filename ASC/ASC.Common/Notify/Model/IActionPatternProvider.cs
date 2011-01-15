#region usings

using ASC.Notify.Engine;
using ASC.Notify.Patterns;

#endregion

namespace ASC.Notify.Model
{
    public delegate IPattern GetPatternCallback(INotifyAction action, string senderName, NotifyRequest request);

    public interface IActionPatternProvider
    {
        GetPatternCallback GetPatternMethod { get; }
        IPattern GetPattern(INotifyAction action, string senderName);

        IPattern GetPattern(INotifyAction action);
    }
}