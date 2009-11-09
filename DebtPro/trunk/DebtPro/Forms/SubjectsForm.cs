using System.Collections.Generic;
using System.Windows.Forms;
using CI.Debt.Domain;

namespace CI.Debt.Forms {

	partial class SubjectsForm : Form, ISubjectsView {

		private bool firstOpen;

		public SubjectsForm() {
			InitializeComponent();
			firstOpen = true;
			subjectsView.AutoGenerateColumns = false;
		}

		#region ISubjectsView Members

		public void ShowSubjects(IList<Subject> subjects) {
			if (firstOpen) {
				subjectsView.DataSource = new SubjectsBindingList(subjects);
				firstOpen = false;
			}
			this.ShowDialog();
		}

		#endregion
	}	
}