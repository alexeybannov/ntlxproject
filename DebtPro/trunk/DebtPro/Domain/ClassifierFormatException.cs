using System;

namespace CI.Debt.Domain {

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
}
