using System;
using CI.Debt.DAO;
using CI.Debt.Domain;

namespace CI.Debt.Impl {

	class ClassifiersPresenter : IClassifiersPresenter {

		private IClassifiersView view;

		public ClassifiersPresenter(IClassifiersView view) {
			this.view = view;
			this.view.ClassifierSelected += OnClassifierSelected;
		}

		private void OnClassifierSelected(object sender, EventArgs e) {
			if (ClassifierSelected != null) ClassifierSelected(this, e);
		}

		#region IClassifiersPresenter Members

		public void ShowClassifiers() {
			view.ShowClassifiers(DebtDAO.GetClassifiers());
		}

		public Classifier SelectedClassifier {
			get { return view.SelectedClassifier; }
			set { view.SelectedClassifier = value; }
		}

		public event EventHandler ClassifierSelected;

		#endregion
	}
}