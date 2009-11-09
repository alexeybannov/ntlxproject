using System;

namespace CI.Debt.Domain {

	/// <summary>
	/// Интерфейс презентера классификаторов.
	/// Подробнее о MVP см. http://www.rsdn.ru/article/patterns/ModelViewPresenter.xml
	/// </summary>
	interface IClassifiersPresenter {

		/// <summary>
		/// Показать все классификаторы.
		/// </summary>
		void ShowClassifiers();

		/// <summary>
		/// Текущий выбранный классификатор.
		/// </summary>
		Classifier SelectedClassifier {
			get;
			set;
		}

		/// <summary>
		/// Событие возникает при выборе текущего классфикатора.
		/// </summary>
		event EventHandler ClassifierSelected;
	}
}
