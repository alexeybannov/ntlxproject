
namespace CI.Debt.Domain {

	/// <summary>
	/// Субъект бюджетного процесса.
	/// </summary>
	class Subject {

		/// <summary>
		/// Идентификатор субъекта.
		/// </summary>
		public virtual long Id {
			get;
			private set;
		}

		/// <summary>
		/// Название субъекта.
		/// </summary>
		public virtual string Name {
			get;
			set;
		}

		/// <summary>
		/// Код субъекта.
		/// </summary>
		public virtual string Code {
			get;
			set;
		}

		/// <summary>
		/// Название бюджета.
		/// </summary>
		public virtual string BudgetName {
			get;
			set;
		}

		/// <summary>
		/// Ссылка на себя.
		/// </summary>
		public virtual Subject Self {
			get { return this; }
		}

		/// <summary>
		/// Полное название субъекта.
		/// </summary>
		public virtual string FullName {
			get { return ToString(); }
		}

		/// <inheritdoc/>
		public override bool Equals(object obj) {
			var subj = obj as Subject;
			return subj != null && Id == subj.Id;
		}

		/// <inheritdoc/>
		public override int GetHashCode() {
			return Id.GetHashCode();
		}

		/// <inheritdoc/>
		public override string ToString() {
			return string.Format(
				"[{0}] {1}{2}",
				Code,
				Name,
				(!string.IsNullOrEmpty(BudgetName) ? string.Format("({0} бюджет)", BudgetName) : string.Empty)
			);
		}
	}
}
