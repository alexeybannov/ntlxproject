
namespace CI.Debt.Domain {

	/// <summary>
	/// Интерфейс представления строк задолженности.
	/// Подробнее о MVP см. http://www.rsdn.ru/article/patterns/ModelViewPresenter.xml
	/// </summary>
	interface IDebtRowPresenter {

		/// <summary>
		/// Отобразить строки задолженности.
		/// </summary>
		void ShowDebtRows();
	}
}
