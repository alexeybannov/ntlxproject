
namespace CI.Debt.Domain {

	/// <summary>
	/// Интерфейс презентера субъектов.
	/// Подробнее о MVP см. http://www.rsdn.ru/article/patterns/ModelViewPresenter.xml
	/// </summary>
	interface ISubjectsPresenter {

		/// <summary>
		/// Отобразить все субъекты.
		/// </summary>
		void ShowSubjects();
	}
}
