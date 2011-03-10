using System;
using System.Collections.Generic;
using ASC.Core.Users;
using ASC.Notify.Recipients;

namespace ASC.Core.Notify
{
    public class RecipientProviderImpl : IRecipientProvider
    {
        public virtual IRecipient GetRecipient(string id)
        {
            var recID = Guid.Empty;
            if (TryParseGuid(id, out recID))
            {
                var user = CoreContext.UserManager.GetUsers(recID);
                if (user.ID != Constants.LostUser.ID) return new DirectRecipient(user.ID.ToString(), user.ToString());

                var group = CoreContext.GroupManager.GetGroupInfo(recID);
                if (group.ID != Constants.LostGroupInfo.ID) return new RecipientsGroup(group.ID.ToString(), group.Name);
            }
            return null;
        }

        public virtual IRecipient[] GetGroupEntries(IRecipientsGroup group)
        {
            if (group == null) throw new ArgumentNullException("group");

            var result = new List<IRecipient>();
            var groupID = Guid.Empty;
            if (TryParseGuid(group.ID, out groupID))
            {
                var coreGroup = CoreContext.GroupManager.GetGroupInfo(groupID);
                if (coreGroup.ID != Constants.LostGroupInfo.ID)
                {
                    foreach (var gr in coreGroup.Descendants)
                    {
                        result.Add(new RecipientsGroup(gr.ID.ToString(), gr.Name));
                    }
                    var users = CoreContext.UserManager.GetUsersByGroup(coreGroup.ID);
                    Array.ForEach(users, u => result.Add(new DirectRecipient(u.ID.ToString(), u.ToString())));
                }
            }
            return result.ToArray();
        }

        public virtual IRecipientsGroup[] GetGroups(IRecipient recipient)
        {
            if (recipient == null) throw new ArgumentNullException("recipient");

            var result = new List<IRecipientsGroup>();
            var recID = Guid.Empty;
            if (TryParseGuid(recipient.ID, out recID))
            {
                if (recipient is IRecipientsGroup)
                {
                    var group = CoreContext.GroupManager.GetGroupInfo(recID);
                    while (group != null && group.Parent != null)
                    {
                        result.Add(new RecipientsGroup(group.Parent.ID.ToString(), group.Parent.Name));
                        group = group.Parent;
                        break;
                    }
                }
                else if (recipient is IDirectRecipient)
                {
                    foreach (var group in CoreContext.UserManager.GetUserGroups(recID, IncludeType.Distinct))
                    {
                        result.Add(new RecipientsGroup(group.ID.ToString(), group.Name));
                    }
                }
            }
            return result.ToArray();
        }

        public virtual string[] GetRecipientAddresses(IDirectRecipient recipient, string senderName)
        {
            if (recipient == null) throw new ArgumentNullException("recipient");
            var userID = Guid.Empty;
            if (TryParseGuid(recipient.ID, out userID))
            {
                var user = CoreContext.UserManager.GetUsers(userID);
                if (user.ID != Constants.LostUser.ID)
                {
                    if (senderName == ASC.Core.Configuration.Constants.NotifyEMailSenderSysName) return new[] { user.Email };
                    if (senderName == ASC.Core.Configuration.Constants.NotifyMessengerSenderSysName) return new[] { user.UserName };
                }
            }
            return new string[0];
        }


        public IRecipient GetRecipient(string id, string objectID)
        {
            return GetRecipient(id);
        }

        public IRecipient[] GetGroupEntries(IRecipientsGroup group, string objectID)
        {
            return GetGroupEntries(group);
        }

        public IRecipientsGroup[] GetGroups(IRecipient recipient, string objectID)
        {
            return GetGroups(recipient);
        }

        public string[] GetRecipientAddresses(IDirectRecipient recipient, string senderName, string objectID)
        {
            return GetRecipientAddresses(recipient, senderName);
        }


        private bool TryParseGuid(string id, out Guid guid)
        {
            guid = Guid.Empty;
            if (!string.IsNullOrEmpty(id))
            {
                try
                {
                    guid = new Guid(id);
                    return true;
                }
                catch (FormatException) { }
                catch (OverflowException) { }
            }
            return false;
        }
    }
}
