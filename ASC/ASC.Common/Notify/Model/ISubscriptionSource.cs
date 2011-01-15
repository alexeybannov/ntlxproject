#region usings

using ASC.Notify.Recipients;

#endregion

namespace ASC.Notify.Model
{
    public interface ISubscriptionSource
    {
        string[] GetSubscriptionMethod(INotifyAction action, IRecipient recipient);

        IRecipient[] GetRecipients(INotifyAction action, string objectID);

        bool IsUnsubscribe(IDirectRecipient recipient, INotifyAction action, string objectID);
    }
}