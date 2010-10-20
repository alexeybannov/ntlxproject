using System.Xml.Linq;

namespace NXmlConnector.Model.Commands
{
    abstract class Command
    {
        public string Id
        {
            get;
            protected set;
        }

        protected Command(string id)
        {
            Id = id;
        }


        public override string ToString()
        {
            var document = new XDocument();
            var element = new XElement("command", new XAttribute("id", Id));
            document.AddFirst(element);

            WriteElement(element);

            return document.ToString(SaveOptions.DisableFormatting);
        }

        protected virtual void WriteElement(XElement command)
        {

        }
    }
}
