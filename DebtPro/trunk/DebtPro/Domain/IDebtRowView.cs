using System;
using System.Collections.Generic;

namespace CI.Debt.Domain {

	/// <summary>
	/// Интерфейс представления классификаторов.
	/// Подробнее о MVP см. http://www.rsdn.ru/article/patterns/ModelViewPresenter.xml
	/// </summary>
	interface IDebtRowView {

		/// <summary>
		/// Тип задолженности.
		/// </summary>
		DebtType DebtType {
			get;
			set;
		}

		/// <summary>
		/// Месяц задолженности.
		/// </summary>
		int Month {
			get;
			set;
		}

		/// <summary>
		/// Год задолженности.
		/// </summary>
		int Year {
			get;
			set;
		}

		/// <summary>
		/// Отобразить список строк задолженности.
		/// </summary>
		/// <param name="rows"></param>
		void ShowDebtRows(IList<DebtRow> rows);

		/// <summary>
		/// Событие возникает при изменения одного из свойств строки задолженности
		/// (тип, месяц или год).
		/// </summary>
		event EventHandler DebtRowViewPropertiesChanged;
	}
}
