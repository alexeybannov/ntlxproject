using System;
using System.ComponentModel;
using CI.Debt.DAO;

namespace CI.Debt.Domain {

	/// <summary>
	/// Код бюджетной классификации расходов.
	/// </summary>
	class Classifier : IComparable<Classifier> {

		/// <summary>
		/// Маска бюджетного классификатора расходов.
		/// </summary>
		public static readonly string Mask = @"000\.00 00\.000 00 00\.000\.000\.000\:000";

		/// <summary>
		/// Длина кода классификатора.
		/// </summary>
		public static readonly int CodeLenght = 26;

		private string code;

		private static readonly MaskedTextProvider MaskedTextProvider;

		/// <summary>
		/// Идентификатор классификатора.
		/// </summary>
		public virtual long Id {
			get;
			set;
		}

		/// <summary>
		/// Тип классификатора.
		/// </summary>
		public virtual int ClsfType {
			get;
			private set;
		}

		/// <summary>
		/// Код классификатора.
		/// </summary>
		public virtual string Code {
			get { return code; }
			set { SetCode(value); }
		}

		/// <summary>
		/// Код классификатора по маске.
		/// </summary>
		public virtual string MaskedCode {
			get {
				lock (MaskedTextProvider) {
					MaskedTextProvider.Set(Code != null ? Code : string.Empty);
					return MaskedTextProvider.ToDisplayString();
				}
			}
			set {
				lock (MaskedTextProvider) {
					MaskedTextProvider.Set(value != null ? value : string.Empty);
					try {
						MaskedTextProvider.IncludeLiterals = false;
						Code = MaskedTextProvider.ToString();
					}
					finally {
						MaskedTextProvider.IncludeLiterals = true;
					}
				}
			}
		}

		/// <summary>
		/// Наименование классификатора.
		/// </summary>
		public virtual string Name {
			get;
			set;
		}

		/// <summary>
		/// Полное наименование классификатора.
		/// </summary>
		public virtual string FullName {
			get;
			set;
		}

		/// <summary>
		/// Код бюджетной классификации.
		/// </summary>
		public virtual string BudgetCode {
			get;
			private set;
		}

		/// <summary>
		/// Экономическая статья.
		/// </summary>
		public virtual string EconomicalItemCode {
			get;
			private set;
		}

		/// <summary>
		/// Код классификации группы №1.
		/// </summary>
		public virtual string GrpCode01 {
			get;
			private set;
		}

		/// <summary>
		/// Наименование классификации группы №1.
		/// </summary>
		public virtual string GrpName01 {
			get;
			set;
		}

		/// <summary>
		/// Код классификации группы №2.
		/// </summary>
		public virtual string GrpCode02 {
			get;
			private set;
		}

		/// <summary>
		/// Наименование классификации группы №2.
		/// </summary>
		public virtual string GrpName02 {
			get;
			set;
		}

		/// <summary>
		/// Код классификации группы №3.
		/// </summary>
		public virtual string GrpCode03 {
			get;
			private set;
		}

		/// <summary>
		/// Наименование классификации группы №3.
		/// </summary>
		public virtual string GrpName03 {
			get;
			set;
		}

		/// <summary>
		/// Код классификации группы №4.
		/// </summary>
		public virtual string GrpCode04 {
			get;
			private set;
		}

		/// <summary>
		/// Наименование классификации группы №4.
		/// </summary>
		public virtual string GrpName04 {
			get;
			set;
		}

		/// <summary>
		/// Код классификации группы №5.
		/// </summary>
		public virtual string GrpCode05 {
			get;
			private set;
		}

		/// <summary>
		/// Наименование классификации группы №5.
		/// </summary>
		public virtual string GrpName05 {
			get;
			set;
		}

		/// <summary>
		/// Код классификации группы №6.
		/// </summary>
		public virtual string GrpCode06 {
			get;
			private set;
		}

		/// <summary>
		/// Наименование классификации группы №6.
		/// </summary>
		public virtual string GrpName06 {
			get;
			set;
		}

		/// <summary>
		/// Код классификации группы №7.
		/// </summary>
		public virtual string GrpCode07 {
			get;
			private set;
		}

		/// <summary>
		/// Наименование классификации группы №7.
		/// </summary>
		public virtual string GrpName07 {
			get;
			set;
		}

		/// <summary>
		/// Код классификации группы №8.
		/// </summary>
		public virtual string GrpCode08 {
			get;
			private set;
		}

		/// <summary>
		/// Наименование классификации группы №8.
		/// </summary>
		public virtual string GrpName08 {
			get;
			set;
		}

		/// <summary>
		/// Код классификации группы №9.
		/// </summary>
		public virtual string GrpCode09 {
			get;
			private set;
		}

		/// <summary>
		/// Наименование классификации группы №9.
		/// </summary>
		public virtual string GrpName09 {
			get;
			set;
		}

		/// <summary>
		/// Код классификации группы №10.
		/// </summary>
		public virtual string GrpCode10 {
			get;
			private set;
		}

		/// <summary>
		/// Наименование классификации группы №10.
		/// </summary>
		public virtual string GrpName10 {
			get;
			set;
		}

		/// <summary>
		/// Код классификации группы №11.
		/// </summary>
		public virtual string GrpCode11 {
			get;
			private set;
		}

		/// <summary>
		/// Наименование классификации группы №11.
		/// </summary>
		public virtual string GrpName11 {
			get;
			set;
		}

		/// <summary>
		/// Код классификации группы №12.
		/// </summary>
		public virtual string GrpCode12 {
			get;
			private set;
		}

		/// <summary>
		/// Наименование классификации группы №12.
		/// </summary>
		public virtual string GrpName12 {
			get;
			set;
		}

		/// <summary>
		/// Конструктор типа.
		/// </summary>
		static Classifier() {
			MaskedTextProvider = new MaskedTextProvider(Mask);
		}

		/// <summary>
		/// Создает экземпляр объекта Classifier.
		/// </summary>
		public Classifier() {
			ClsfType = 1;
		}

		/// <summary>
		/// Создает экземпляр объекта Classifier.
		/// <param name="id">Идентификатор классификатора.</param>
		/// </summary>
		public Classifier(long id)
			: this() {
			Id = id;
		}

		/// <inheritdoc/>
		public override bool Equals(object obj) {
			var clsf = obj as Classifier;
			return clsf != null && Id == clsf.Id;
		}

		/// <inheritdoc/>
		public override int GetHashCode() {
			return Id.GetHashCode();
		}

		/// <inheritdoc/>
		public override string ToString() {
			return string.Format("{0}", MaskedCode);
		}

		private void SetCode(string code) {
			if (string.IsNullOrEmpty(code)) throw new ClassifierFormatException("Код классификатора не может быть пустым.");
			if (code.Length != CodeLenght) throw new ClassifierFormatException(string.Format("Код классификатора должен быть длиной в {0} символов.", CodeLenght));
			lock (MaskedTextProvider) {
				MaskedTextProvider.Set(code);
				if (!MaskedTextProvider.MaskFull) throw new ClassifierFormatException("Код классификатора не удовлетворяет маске классификатора.");
			}

			this.code = code;
			BudgetCode = this.code.Substring(0, 17);
			EconomicalItemCode = this.code.Substring(20, 3);
			GrpCode01 = this.code.Substring(0, 3);
			GrpCode02 = this.code.Substring(3, 2);
			GrpCode03 = this.code.Substring(5, 2);
			GrpCode04 = this.code.Substring(7, 3);
			GrpCode05 = this.code.Substring(10, 2);
			GrpCode06 = this.code.Substring(12, 2);
			GrpCode07 = this.code.Substring(14, 3);
			GrpCode08 = this.code.Substring(17, 3);
			GrpCode09 = this.code.Substring(20, 1);
			GrpCode10 = this.code.Substring(21, 1);
			GrpCode11 = this.code.Substring(22, 1);
			GrpCode12 = this.code.Substring(23, 3);
		}

		#region IComparable<Classifier> Members

		/// <summary>
		/// Метод сравнивает два классификатора.
		/// </summary>
		/// <param name="other">Классификатор</param>
		/// <returns>0, если коды классификаторов равны, -1, если код текущего классификатора 
		/// меньше сравниваемого и 1, если код текущего классификатора больше сравниваемого.</returns>
		public int CompareTo(Classifier other) {
			return string.Compare(Code, other.Code);
		}

		#endregion
	}

	/// <summary>
	/// Тип определяет исключение неверного кода классификатора.
	/// </summary>
	class ClassifierFormatException : FormatException {

		/// <inheritdoc/>
		public ClassifierFormatException() : base() { }

		/// <inheritdoc/>
		public ClassifierFormatException(string message) : base(message) { }

		/// <inheritdoc/>
		public ClassifierFormatException(string message, Exception innerException) : base(message, innerException) { }
	}

	/// <summary>
	/// Тип определяет исключение возникающее при поиске несуществующего классификатора.
	/// </summary>
	class ClassifierNotFoundException : Exception {

		/// <inheritdoc/>
		public ClassifierNotFoundException() : base() { }

		/// <inheritdoc/>
		public ClassifierNotFoundException(string message) : base(message) { }

		/// <inheritdoc/>
		public ClassifierNotFoundException(string message, Exception innerException) : base(message, innerException) { }
	}
}