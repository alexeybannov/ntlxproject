using System.Xml.Linq;

namespace NXmlConnector.Model.Commands
{
    class CommandChangePassword : Command
    {
        public string OldPassword
        {
            get;
            private set;
        }

        public string NewPassword
        {
            get;
            private set;
        }


        public CommandChangePassword(string oldPassword, string newPassword)
            : base("change_pass")
        {
            OldPassword = oldPassword;
            NewPassword = newPassword;
        }

        protected override void WriteElement(XElement command)
        {
            command.Add(new XElement("oldpass", OldPassword));
            command.Add(new XElement("newpass", NewPassword));
        }
    }
}
