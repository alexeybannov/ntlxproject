using System.Xml.Serialization;

namespace CI.Debt.Xml {

	[XmlRoot(ElementName = "Debt")]
	public class XmlClsfRows {

		[XmlArrayItem(ElementName = "Row")]
		public XmlClsfRow[] Rows {
			get;
			set;
		}
	}
}
