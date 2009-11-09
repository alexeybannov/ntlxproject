using System.Collections.Generic;

namespace CI.Debt.Domain {

	/// <summary>
	/// Интерфейс представления субъектов.
	/// Подробнее о MVP см. http://www.rsdn.ru/article/patterns/ModelViewPresenter.xml
	/// </summary>
	interface ISubjectsView {

		/// <summary>
		/// Отобразить список субъектов.
		/// </summary>
		/// <param name="subjects">Список субъектов</param>
		void ShowSubjects(IList<Subject> subjects);
	}
}
