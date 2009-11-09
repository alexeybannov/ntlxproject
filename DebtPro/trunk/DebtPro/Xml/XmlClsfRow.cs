using System.Xml.Serialization;

namespace CI.Debt.Xml {

	public class XmlClsfRow {

		[XmlAttribute]
		public string ClassifierCode {
			get;
			set;
		}

		[XmlAttribute]
		public long ClassifierId {
			get;
			set;
		}
	}
}