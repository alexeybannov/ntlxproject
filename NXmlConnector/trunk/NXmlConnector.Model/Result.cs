using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    [XmlRoot("result")]
    public class Result
    {
        [XmlAttribute("success")]
        public bool Success
        {
            get;
            set;
        }

        [XmlElement("message")]
        public string ErrorText
        {
            get;
            set;
        }

        [XmlAttribute("transactionid")]
        public int TransactionId
        {
            get;
            set;
        }

        [XmlAttribute("diff")]
        public int Difference
        {
            get;
            set;
        }
    }
}
