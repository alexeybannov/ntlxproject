#region usings

using System;
using System.Runtime.Remoting;
using System.Runtime.Serialization;

#endregion

namespace ASC.Common.Security.Authorizing
{
    [Serializable]
    public class AuthorizingException : RemotingException
    {
        private readonly string _Message;

        public AuthorizingException(string message) : base(message)
        {
        }

        public AuthorizingException(ISubject subject, IAction[] actions)
        {
            if (subject == null) throw new ArgumentNullException("subject");
            if (actions == null || actions.Length == 0) throw new ArgumentNullException("actions");
            Subject = subject;
            Actions = actions;
            string sactions = "";
            Array.ForEach(actions, action => { sactions += action.ToString() + ", "; });
            _Message = String.Format(
                CommonDescriptionResource.AuthorizingException_Message,
                subject,
                sactions
                );
        }

        public AuthorizingException(ISubject subject, IAction[] actions, ISubject[] denySubjects, IAction[] denyActions)
        {
            _Message = FormatErrorMessage(subject, actions, denySubjects, denyActions);
        }

        protected AuthorizingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            _Message = info.GetValue("_Message", typeof (string)) as string;
            Subject = info.GetValue("Subject", typeof (ISubject)) as ISubject;
            Actions = info.GetValue("Actions", typeof (IAction[])) as IAction[];
        }

        public override string Message
        {
            get { return _Message; }
        }

        public ISubject Subject { get; internal set; }
        public IAction[] Actions { get; internal set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("Subject", Subject, typeof (ISubject));
            info.AddValue("_Message", _Message, typeof (string));
            info.AddValue("Actions", Actions, typeof (IAction[]));
            base.GetObjectData(info, context);
        }

        internal static string FormatErrorMessage(ISubject subject, IAction[] actions, ISubject[] denySubjects,
                                                  IAction[] denyActions)
        {
            if (subject == null) throw new ArgumentNullException("subject");
            if (actions == null || actions.Length == 0) throw new ArgumentNullException("actions");
            if (denySubjects == null || denySubjects.Length == 0) throw new ArgumentNullException("denySubjects");
            if (denyActions == null || denyActions.Length == 0) throw new ArgumentNullException("denyActions");
            if (actions.Length != denySubjects.Length || actions.Length != denyActions.Length)
                throw new ArgumentException();
            string reasons = "";
            for (int i = 0; i < actions.Length; i++)
            {
                string reason = "";
                if (denySubjects[i] != null && denyActions[i] != null)
                    reason = String.Format(CommonDescriptionResource.AuthorizingException_MessageEx_ReasonFormat_Deny,
                                           actions[i].Name,
                                           (denySubjects[i] is IRole ? "role:" : "") + denySubjects[i].Name,
                                           denyActions[i].Name
                        );
                else
                    reason = String.Format(CommonDescriptionResource.AuthorizingException_MessageEx_ReasonFormat_Empty,
                                           actions[i].Name);
                if (i != actions.Length - 1)
                    reason += ", ";
                reasons += reason;
            }
            string sactions = "";
            Array.ForEach(actions, action => { sactions += action.ToString() + ", "; });
            string message = String.Format(
                CommonDescriptionResource.AuthorizingException_MessageEx,
                (subject is IRole ? "role:" : "") + subject.Name,
                sactions,
                reasons
                );
            return message;
        }
    }
}