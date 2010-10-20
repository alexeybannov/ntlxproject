using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    [XmlRoot("leverage_control")]
    public class LeverageControl
    {
        [XmlAttribute("client")]
        public string Client
        {
            get;
            set;
        }

        [XmlAttribute("leverage_plan")]
        public string LeveragePlan
        {
            get;
            set;
        }

        [XmlAttribute("leverage_fact")]
        public string LeverageFact
        {
            get;
            set;
        }

        [XmlElement("security")]
        public LeverageControlSecurity Securities
        {
            get;
            set;
        }
    }
}