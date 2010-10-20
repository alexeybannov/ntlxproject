using System.Xml.Serialization;

namespace NXmlConnector.Model
{
    [XmlRoot("positions")]
    public class Positions
    {
        [XmlElement("money_position")]
        public MoneyPosition[] MoneyPositions
        {
            get;
            set;
        }

        [XmlElement("sec_position")]
        public SecPosition[] SecPositions
        {
            get;
            set;
        }

        [XmlElement("forts_position")]
        public FortsPosition[] FortsPositions
        {
            get;
            set;
        }

        [XmlElement("forts_money")]
        public FortsMoney[] FortsMoneys
        {
            get;
            set;
        }

        [XmlElement("forts_collaterals")]
        public FortsCollateral[] FortsCollaterals
        {
            get;
            set;
        }

        [XmlElement("spot_limit")]
        public SpotLimit[] SpotLimits
        {
            get;
            set;
        }
    }
}