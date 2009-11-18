
namespace CI.Debt.Domain {

	class DebtSettings {

		internal virtual int Id {
			get;
			set;
		}

		public virtual Subject DefaultSubject {
			get;
			set;
		}

		public virtual bool IsAutoPasteClassifier {
			get;
			set;
		}

		public virtual string FilterBudget {
			get;
			set;
		}

		public DebtSettings() {
			Id = 1;
			IsAutoPasteClassifier = true;
		}

		public override int GetHashCode() {
			return Id.GetHashCode();
		}

		public override bool Equals(object obj) {
			var s = obj as DebtSettings;
			return s != null && s.Id == Id;
		}
	}
}
