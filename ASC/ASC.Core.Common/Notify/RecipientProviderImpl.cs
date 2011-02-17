#region usings

using System;
using System.Collections.Generic;
using ASC.Core.Common;
using ASC.Core.Users;
using ASC.Notify.Recipients;

#endregion

namespace ASC.Core.Notify
{
    public class RecipientProviderImpl
        : IRecipientProvider
    {
        #region IRecipientProvider

        public virtual IRecipient GetRecipient(string id)
        {
            Guid recID = Guid.Empty;
            try
            {
                recID = new Guid(id);
            }
            catch (Exception)
            {
                return null;
            }

            UserInfo user = null;
            user = CoreContext.UserManager.GetUsers(recID);
            if (user == Constants.LostUser)
                user = null;

            if (user != null)
                return new DirectRecipient(user.ID.ToString(), user.ToString());

            GroupInfo coreGroup = CoreContext.GroupManager.GetGroupInfo(recID);
            if (Constants.LostGroupInfo.Equals(coreGroup))
                coreGroup = null;
            else if (coreGroup != null)
                return new RecipientsGroup(coreGroup.ID.ToString(), coreGroup.Name);
            return null;
        }

        public virtual IRecipient[] GetGroupEntries(IRecipientsGroup group)
        {
            if (group == null) throw new ArgumentNullException("group");
            var result = new List<IRecipient>();
            Guid groupID = Guid.Empty;
            try
            {
                groupID = new Guid(group.ID);
            }
            catch (Exception)
            {
                return null;
            }
            GroupInfo coreGroup = CoreContext.GroupManager.GetGroupInfo(groupID);
            if (coreGroup != null)
            {
                if (coreGroup.Descendants != null)
                {
                    foreach (GroupInfo gr in coreGroup.Descendants)
                        result.Add(new RecipientsGroup(gr.ID.ToString(), gr.Name));
                }

                UserInfo[] users = CoreContext.UserManager.GetUsersByGroup(coreGroup.ID);
                Array.ForEach(users, u => result.Add(new DirectRecipient(u.ID.ToString(), u.ToString())));
            }
            return result.ToArray();
        }

        public virtual IRecipientsGroup[] GetGroups(IRecipient recipient)
        {
            if (recipient == null)
                throw new ArgumentNullException("recipient");
            Guid recID = Guid.Empty;
            try
            {
                recID = new Guid(recipient.ID);
            }
            catch (Exception)
            {
                return null;
            }
            var result = new List<IRecipientsGroup>();
            if (recipient is IRecipientsGroup)
            {
                GroupInfo group = CoreContext.GroupManager.GetGroupInfo(recID);
                while (group != null && group.Parent != null)
                {
                    result.Add(new RecipientsGroup(group.Parent.ID.ToString(), group.Parent.Name));
                    group = group.Parent;
                    break;
                }
            }
            else if (recipient is IDirectRecipient)
            {
                GroupInfo[] groups = CoreContext.UserManager.GetUserGroups(recID, IncludeType.Distinct);
                foreach (GroupInfo group in groups)
                {
                    result.Add(new RecipientsGroup(group.ID.ToString(), group.Name));
                }
            }
            return result.ToArray();
        }

        public virtual string[] GetRecipientAddresses(IDirectRecipient recipient, string senderName)
        {
            if (recipient == null)
                throw new ArgumentNullException("recipient");
            Guid userID = Guid.Empty;
            try
            {
                userID = new Guid(recipient.ID);
            }
            catch (Exception)
            {
                throw new ArgumentException("group");
            }
            UserInfo user = CoreContext.UserManager.GetUsers(userID);
            if (senderName == ASC.Core.Configuration.Constants.NotifyEMailSenderSysName)
                return new[] {user.Email};
            else if (senderName == ASC.Core.Configuration.Constants.NotifyMessengerSenderSysName)
                return new[] {user.UserName};
            else
                return new string[] {};
        }

        #endregion

        #region IRecipientProvider the object

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

        #endregion
    }
}