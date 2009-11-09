using System;
using CI.Debt.DAO;
using CI.Debt.Domain;

namespace CI.Debt.Impl {

	class ClsfPresenterTest : IClassifiersPresenter {

		public ClsfPresenterTest() { }

		private void OnClassifierSelected(object sender, EventArgs e) {
			if (ClassifierSelected != null) ClassifierSelected(this, e);
		}

		#region IClassifiersPresenter Members

		public void ShowClassifiers() {
			SelectedClassifier = DebtDAO.EmptyClassifier;
			OnClassifierSelected(this, EventArgs.Empty);
		}

		public Classifier SelectedClassifier {
			get;
			set;
		}

		public event EventHandler ClassifierSelected;

		#endregion
	}
}