namespace ASC.Notify.Recipients
{
    public interface IRecipientProvider
    {
        IRecipient GetRecipient(string id);

        IRecipient[] GetGroupEntries(IRecipientsGroup group);

        IRecipientsGroup[] GetGroups(IRecipient recipient);

        string[] GetRecipientAddresses(IDirectRecipient recipient, string senderName);

        IRecipient[] GetGroupEntries(IRecipientsGroup group, string objectID);

        string[] GetRecipientAddresses(IDirectRecipient recipient, string senderName, string objectID);
    }
}