using CI.Debt.Domain;

namespace CI.Debt.Reports {

	class Report1Row {

		private DebtRow row;

		#region Classifier

		public virtual string GrpCode01 {
			get { return row.Classifier.GrpCode01; }
		}

		public virtual string GrpName01 {
			get { return row.Classifier.GrpName01; }
		}

		public virtual string GrpCode02 {
			get { return row.Classifier.GrpCode02; }
		}

		public virtual string GrpName02 {
			get { return row.Classifier.GrpName02; }
		}

		public virtual string GrpCode03 {
			get { return row.Classifier.GrpCode03; }
		}

		public virtual string GrpName03 {
			get { return row.Classifier.GrpName03; }
		}

		public virtual string GrpCode04 {
			get { return row.Classifier.GrpCode04; }
		}

		public virtual string GrpName04 {
			get { return row.Classifier.GrpName04; }
		}

		public virtual string GrpCode05 {
			get { return row.Classifier.GrpCode05; }
		}

		public virtual string GrpName05 {
			get { return row.Classifier.GrpName05; }
		}

		public virtual string GrpCode06 {
			get { return row.Classifier.GrpCode06; }
		}

		public virtual string GrpName06 {
			get { return row.Classifier.GrpName06; }
		}

		public virtual string GrpCode07 {
			get { return row.Classifier.GrpCode07; }
		}

		public virtual string GrpName07 {
			get { return row.Classifier.GrpName07; }
		}

		public virtual string GrpCode08 {
			get { return row.Classifier.GrpCode08; }
		}

		public virtual string GrpName08 {
			get { return row.Classifier.GrpName08; }
		}

		public virtual string GrpCode09 {
			get { return row.Classifier.GrpCode09; }
		}

		public virtual string GrpName09 {
			get { return row.Classifier.GrpName09; }
		}

		public virtual string GrpCode10 {
			get { return row.Classifier.GrpCode10; }
		}

		public virtual string GrpName10 {
			get { return row.Classifier.GrpName10; }
		}

		public virtual string GrpCode11 {
			get { return row.Classifier.GrpCode11; }
		}

		public virtual string GrpName11 {
			get { return row.Classifier.GrpName11; }
		}

		public virtual string GrpCode12 {
			get { return row.Classifier.GrpCode12; }
		}

		public virtual string GrpName12 {
			get { return row.Classifier.GrpName12; }
		}

		#endregion

		#region Subject

		public string SubjectName {
			get { return row.Subject.Name; }
		}

		#endregion

		#region Amounts

		public double Amount {
			get { return row.Amount; }
		}

		public double Amount2 {
			get { return row.Amount2; }
		}

		#endregion

		/// <summary>
		/// Создает экземпляр объекта Report1Row.
		/// </summary>
		public Report1Row(DebtRow row) {
			this.row = row;
		}

		public override string ToString() {
			return string.Format("[{0}] {1} ({2}) {3}", row.Classifier.MaskedCode, Amount, Amount2, SubjectName);
		}
	}
}
