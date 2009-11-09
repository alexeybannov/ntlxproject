using System;
using System.Text;

namespace CI.Debt.Domain {

	/// <summary>
	/// Код бюджетной классификации расходов.
	/// </summary>
	class Classifier : IComparable<Classifier>, IComparable, IEquatable<Classifier>, IFormattable {

		/// <summary>
		/// Маска бюджетного классификатора расходов.
		/// </summary>
		public static readonly string Mask = "000.00 00.000 00 00.000.000.000:000";

		/// <summary>
		/// Длина кода классификатора.
		/// </summary>
		public static readonly int CodeLenght = 26;

		private string code;

		private string maskedCode;

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
			get { return maskedCode; }
			set { SetCode(value); }
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
		/// Создает экземпляр объекта Classifier.
		/// </summary>
		public Classifier() {
			ClsfType = 1;
			Code = new string('0', CodeLenght);
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

		private void SetCode(string code) {
			if (string.IsNullOrEmpty(code)) throw new ClassifierFormatException("Код классификатора не может быть пустым.");

			var newCode = new StringBuilder(CodeLenght);
			foreach (var c in code) {
				if (char.IsDigit(c)) newCode.Append(c);
			}
			if (newCode.Length != CodeLenght) throw new ClassifierFormatException(string.Format("Код классификатора не удовлетворяет маске классификатора. Код: {0}, маска: {1}", code, Mask));
			this.code = newCode.ToString();

			newCode = new StringBuilder(Mask.Length);
			int index = 0;
			foreach (var c in Mask) {
				if (char.IsDigit(c)) {
					newCode.Append(Code[index]);
					index++;
				}
				else {
					newCode.Append(c);
				}
			}
			maskedCode = newCode.ToString();

			BudgetCode = Code.Substring(0, 17);
			EconomicalItemCode = Code.Substring(20, 3);
			GrpCode01 = Code.Substring(0, 3);
			GrpCode02 = Code.Substring(3, 2);
			GrpCode03 = Code.Substring(5, 2);
			GrpCode04 = Code.Substring(7, 3);
			GrpCode05 = Code.Substring(10, 2);
			GrpCode06 = Code.Substring(12, 2);
			GrpCode07 = Code.Substring(14, 3);
			GrpCode08 = Code.Substring(17, 3);
			GrpCode09 = Code.Substring(20, 1);
			GrpCode10 = Code.Substring(21, 1);
			GrpCode11 = Code.Substring(22, 1);
			GrpCode12 = Code.Substring(23, 3);
		}

		#region IComparable<Classifier> Members

		/// <summary>
		/// Метод сравнивает два классификатора.
		/// </summary>
		/// <param name="other">Классификатор</param>
		/// <returns>0, если коды классификаторов равны, -1, если код текущего классификатора 
		/// меньше сравниваемого и 1, если код текущего классификатора больше сравниваемого.</returns>
		public int CompareTo(Classifier other) {
			if (other == null) return 1;
			return string.Compare(Code, other.Code);
		}

		#endregion

		#region IComparable Members

		public virtual int CompareTo(object obj) {
			if (obj == null) return 1;
			if (!(obj is Classifier)) throw new ArgumentException("Тип объекта должен быть Classifier.");
			return string.Compare(Code, ((Classifier)obj).Code);
		}

		#endregion

		#region IEquatable<Classifier> Members

		public virtual bool Equals(Classifier other) {
			return other != null && Id == other.Id;
		}

		#endregion

		#region IFormattable Members

		/// <inheritdoc/>
		public override string ToString() {
			return ToString("M");
		}

		public virtual string ToString(string format) {
			return ToString(format, null);
		}

		public virtual string ToString(string format, IFormatProvider formatProvider) {
			if (string.IsNullOrEmpty(format)) return Code;
			if (format.ToUpperInvariant() == "M") return MaskedCode;
			throw new ClassifierFormatException(string.Format("Неизвестный формат классификатора '{0}'", format));
		}

		#endregion
	}
}