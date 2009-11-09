using System.Collections.Generic;
using System.Xml.Serialization;
using CI.Debt.DAO;
using CI.Debt.Domain;

namespace CI.Debt.Xml {

	[XmlRoot(ElementName = "Debt")]
	public class XmlDebtRows {

		[XmlArrayItem(ElementName = "Row")]
		public XmlDebtRow[] Rows {
			get;
			set;
		}

		public XmlDebtRows() { }

		internal XmlDebtRows(IEnumerable<DebtRow> rows) {
			Rows = new List<DebtRow>(rows)
				.ConvertAll<XmlDebtRow>(r => new XmlDebtRow(r))
				.ToArray();
		}
	}
}
