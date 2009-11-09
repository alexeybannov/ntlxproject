using CI.Debt.Domain;

namespace CI.Debt.Reports {

	class Report1Row {

		private DebtRow row;

		#region Classifier

		public virtual string GrpCode01 {
			get;
			private set;
		}

		public virtual string GrpName01 {
			get { return row.Classifier.GrpName01; }
		}

		public virtual string GrpCode02 {
			get;
			private set;
		}

		public virtual string GrpName02 {
			get { return row.Classifier.GrpName02; }
		}

		public virtual string GrpCode03 {
			get;
			private set;
		}

		public virtual string GrpName03 {
			get { return row.Classifier.GrpName03; }
		}

		public virtual string GrpCode04 {
			get;
			private set;
		}

		public virtual string GrpName04 {
			get {
				return
					row.Classifier.GrpCode06 != "00" ?
						row.Classifier.GrpName06 :
						row.Classifier.GrpCode05 != "00" ?
							row.Classifier.GrpName05 :
							row.Classifier.GrpName04;
			}
		}

		public virtual string GrpCode05 {
			get;
			private set;
		}

		public virtual string GrpName05 {
			get { return row.Classifier.GrpName05; }
		}

		public virtual string GrpCode06 {
			get;
			private set;
		}

		public virtual string GrpName06 {
			get { return row.Classifier.GrpName06; }
		}

		public virtual string GrpCode07 {
			get;
			private set;
		}

		public virtual string GrpName07 {
			get { return row.Classifier.GrpName07; }
		}

		public virtual string GrpCode08 {
			get;
			private set;
		}

		public virtual string GrpName08 {
			get { return row.Classifier.GrpName08; }
		}

		public virtual string GrpCode09 {
			get;
			private set;
		}

		public virtual string GrpName09 {
			get { return row.Classifier.GrpName09; }
		}

		public virtual string GrpCode10 {
			get;
			private set;
		}

		public virtual string GrpName10 {
			get { return row.Classifier.GrpName10; }
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
			var clsf = row.Classifier;
			GrpCode01 = string.Format("{0}.00 00.000 00 00.000.000.000:000", clsf.GrpCode01);
			GrpCode02 = string.Format("{0}.{1} 00.000 00 00.000.000.000:000", clsf.GrpCode01, clsf.GrpCode02);
			GrpCode03 = string.Format("{0}.{1} {2}.000 00 00.000.000.000:000", clsf.GrpCode01, clsf.GrpCode02, clsf.GrpCode03);
			GrpCode04 = string.Format("{0}.{1} {2}.{3} {4} {5}.000.000.000:000", clsf.GrpCode01, clsf.GrpCode02, clsf.GrpCode03,
				clsf.GrpCode04, clsf.GrpCode05, clsf.GrpCode06);
			GrpCode05 = string.Format("{0}.{1} {2}.{3} {4} {5}.{6}.000.000:000", clsf.GrpCode01, clsf.GrpCode02, clsf.GrpCode03,
				clsf.GrpCode04, clsf.GrpCode05, clsf.GrpCode06,
				clsf.GrpCode07);
			GrpCode06 = string.Format("{0}.{1} {2}.{3} {4} {5}.{6}.{7}.000:000", clsf.GrpCode01, clsf.GrpCode02, clsf.GrpCode03,
				clsf.GrpCode04, clsf.GrpCode05, clsf.GrpCode06,
				clsf.GrpCode07, clsf.GrpCode08);
			GrpCode07 = string.Format("{0}.{1} {2}.{3} {4} {5}.{6}.{7}.{8}00:000", clsf.GrpCode01, clsf.GrpCode02, clsf.GrpCode03,
				clsf.GrpCode04, clsf.GrpCode05, clsf.GrpCode06,
				clsf.GrpCode07, clsf.GrpCode08, clsf.GrpCode09);
			GrpCode08 = string.Format("{0}.{1} {2}.{3} {4} {5}.{6}.{7}.{8}{9}0:000", clsf.GrpCode01, clsf.GrpCode02, clsf.GrpCode03,
				clsf.GrpCode04, clsf.GrpCode05, clsf.GrpCode06,
				clsf.GrpCode07, clsf.GrpCode08, clsf.GrpCode09, clsf.GrpCode10);
			GrpCode09 = string.Format("{0}.{1} {2}.{3} {4} {5}.{6}.{7}.{8}{9}{10}:000", clsf.GrpCode01, clsf.GrpCode02, clsf.GrpCode03,
				clsf.GrpCode04, clsf.GrpCode05, clsf.GrpCode06,
				clsf.GrpCode07, clsf.GrpCode08, clsf.GrpCode09, clsf.GrpCode10, clsf.GrpCode11);
			GrpCode10 = clsf.MaskedCode;
		}
	}
}