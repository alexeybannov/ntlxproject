using System;
using CI.Debt.DAO;
using CI.Debt.Domain;

namespace CI.Debt.Impl {

	class DebtRowPresenter : IDebtRowPresenter {

		private IDebtRowView view;

		public DebtRowPresenter(IDebtRowView view) {
			this.view = view;
			this.view.DebtRowViewPropertiesChanged += DebtRowViewPropertiesChanged;
		}

		private void DebtRowViewPropertiesChanged(object sender, EventArgs e) {
			ShowDebtRows();
		}

		public void ShowDebtRows() {
			view.ShowDebtRows(DebtDAO.GetDebtRows(view.DebtType, view.Month, view.Year));
		}
	}
}
