using System.Xml.Serialization;
using CI.Debt.Domain;
using CI.Debt.DAO;

namespace CI.Debt.Xml {

	public class XmlDebtRow {

		[XmlAttribute]
		public string ClassifierCode {
			get;
			set;
		}

		[XmlAttribute]
		public double Amount {
			get;
			set;
		}

		[XmlAttribute]
		public double Amount2 {
			get;
			set;
		}

		[XmlAttribute]
		public string SubjectName {
			get;
			set;
		}

		[XmlAttribute]
		public string SubjectCode {
			get;
			set;
		}

		[XmlAttribute]
		public long ClassifierId {
			get;
			set;
		}

		[XmlAttribute]
		public long SubjectId {
			get;
			set;
		}

		[XmlAttribute]
		public int DebtType {
			get;
			set;
		}

		[XmlAttribute]
		public int Month {
			get;
			set;
		}

		[XmlAttribute]
		public int Year {
			get;
			set;
		}

		protected XmlDebtRow() {
			
		}

		internal XmlDebtRow(DebtRow row) {
			this.Amount = row.Amount;
			this.Amount2 = row.Amount2;
			this.ClassifierCode = row.Classifier.Code;
			this.ClassifierId = row.Classifier.Id;
			this.DebtType = (int)row.DebtType;
			this.Month = row.Month;
			this.SubjectCode = row.Subject.Code;
			this.SubjectId = row.Subject.Id;
			this.SubjectName = row.Subject.Name;
			this.Year = row.Year;
		}
	}
}