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
