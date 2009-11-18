using System;
using System.Windows.Forms;
using CI.Debt.DAO;

namespace CI.Debt.Forms {

	partial class SubjectsForm : Form {

		public SubjectsForm() {
			InitializeComponent();
			subjectsView.AutoGenerateColumns = false;
		}

        private void SubjectsForm_Shown(object sender, EventArgs e)
        {
            subjectsView.DataSource = new SubjectsBindingList(DebtDAO.GetSubjects());
        }
	}	
}