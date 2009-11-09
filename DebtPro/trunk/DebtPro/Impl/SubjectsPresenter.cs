using CI.Debt.DAO;
using CI.Debt.Domain;

namespace CI.Debt.Impl {

	class SubjectsPresenter : ISubjectsPresenter {

		private ISubjectsView view;

		public SubjectsPresenter(ISubjectsView view) {
			this.view = view;
		}

		public void ShowSubjects() {
			view.ShowSubjects(DebtDAO.GetSubjects());
		}
	}
}
