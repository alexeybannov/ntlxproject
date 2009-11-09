using System;
using System.Collections.Generic;

namespace CI.Debt.Domain {

	/// <summary>
	/// Интерфейс представления классификаторов.
	/// Подробнее о MVP см. http://www.rsdn.ru/article/patterns/ModelViewPresenter.xml
	/// </summary>
	interface IClassifiersView {

		/// <summary>
		/// Отобразить классификаторы.
		/// </summary>
		/// <param name="classifiers">Список классификаторов</param>
		void ShowClassifiers(IList<Classifier> classifiers);

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
