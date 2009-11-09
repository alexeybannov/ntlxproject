using System;
using CI.Debt.DAO;

namespace CI.Debt.Domain {

	/// <summary>
	/// Строка задолженности.
	/// </summary>
	class DebtRow : ICloneable {

		private int month;

		private int year;

		/// <summary>
		/// Идентификатор строки.
		/// </summary>
		public virtual int Id {
			get;
			private set;
		}

		/// <summary>
		/// Классификатор.
		/// </summary>
		public virtual Classifier Classifier {
			get;
			set;
		}

		/// <summary>
		/// Субъект.
		/// </summary>
		public virtual Subject Subject {
			get;
			set;
		}

		/// <summary>
		/// Сумма задолженности.
		/// </summary>
		public virtual double Amount {
			get;
			set;
		}

		/// <summary>
		/// Сумма просроченной задолженности.
		/// </summary>
		public virtual double Amount2 {
			get;
			set;
		}

		/// <summary>
		/// Тип задолженности.
		/// </summary>
		public virtual DebtType DebtType {
			get;
			set;
		}

		/// <summary>
		/// Месяц задолженности.
		/// </summary>
		public virtual int Month {
			get { return month; }
			set {
				if (month != value) {
					if (value < 1 || 12 < value) throw new ArgumentOutOfRangeException("month", value, "Месяц должен быть в диапазоне от 1 до 12.");
					month = value;
				}
			}
		}

		/// <summary>
		/// Год задолженности.
		/// </summary>
		public virtual int Year {
			get { return year; }
			set {
				if (year != value) {
					if (value < 1983 || 2025 < value) throw new ArgumentOutOfRangeException("year", value, "Год должен быть в диапазоне от 1983 до 2025.");
					year = value;
				}
			}
		}

		/// <inheritdoc/>
		public override bool Equals(object obj) {
			var row = obj as DebtRow;
			return row != null && Id == row.Id;
		}

		/// <inheritdoc/>
		public override int GetHashCode() {
			return Id.GetHashCode();
		}

		/// <inheritdoc/>
		public override string ToString() {
			return string.Format("Классификатор: {0}, субъект: {1}, сумма: {2}, тип: {3}, месяц: {4}, год: {5}", Classifier, Subject, Amount, DebtType, Month, Year);
		}

		#region ICloneable Members

		public object Clone() {
			return MemberwiseClone();
		}

		#endregion
	}
}