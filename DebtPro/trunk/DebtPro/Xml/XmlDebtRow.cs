using System.Xml.Serialization;
using CI.Debt.Domain;
using CI.Debt.DAO;

namespace CI.Debt.Xml {

	public class XmlDebtRow {

		internal DebtRow Row {
			get;
			private set;
		}

		[XmlAttribute]
		public string ClassifierCode {
			get { return Row.Classifier.Code; }
			set { Row.Classifier = DebtDAO.FindClassifier(value); }
		}

		[XmlAttribute]
		public double Amount {
			get { return Row.Amount; }
			set { Row.Amount = value; }
		}

		[XmlAttribute]
		public double Amount2 {
			get { return Row.Amount2; }
			set { Row.Amount2 = value; }
		}

		[XmlAttribute]
		public string SubjectName {
			get { return Row.Subject.Name; }
			set { }
		}

		[XmlAttribute]
		public string SubjectCode {
			get { return Row.Subject.Code; }
			set { Row.Subject = DebtDAO.GetSubjectByCode(value); }
		}

		[XmlAttribute]
		public long ClassifierId {
			get { return Row.Classifier.Id; }
			set { }
		}

		[XmlAttribute]
		public long SubjectId {
			get { return Row.Subject.Id; }
			set { }
		}

		[XmlAttribute]
		public int DebtType {
			get { return (int)Row.DebtType; }
			set { Row.DebtType = (DebtType)value; }
		}

		[XmlAttribute]
		public int Month {
			get { return Row.Month; }
			set { Row.Month = value; }
		}

		[XmlAttribute]
		public int Year {
			get { return Row.Year; }
			set { Row.Year = value; }
		}

		protected XmlDebtRow() {
			Row = new DebtRow();
		}

		internal XmlDebtRow(DebtRow row) {
			this.Row = row;
		}
	}
}