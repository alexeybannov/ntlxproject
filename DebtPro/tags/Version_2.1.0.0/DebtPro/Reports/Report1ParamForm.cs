using System.Windows.Forms;
using CI.Debt.Domain;
using CI.Debt.DAO;

namespace CI.Debt.Reports {

	partial class Report1ParamForm : Form {

		public bool GroupBySubjects {
			get { return checkBoxSubjectGroup.Checked; }
		}

		public Report1ParamForm(DebtType type, int month, int year) {
			InitializeComponent();
			DebtProperties.DebtType = type;
			DebtProperties.Month = month;
			DebtProperties.Year = year;

			comboBoxBudgetName.Items.Add("<Все бюджеты>");
			comboBoxBudgetName.Items.AddRange(DebtDAO.GetBudgets());
			comboBoxBudgetName.SelectedIndex = 0;
		}

		private void checkBoxSubjectGroup_CheckedChanged(object sender, System.EventArgs e) {
			labelBudgetName.Enabled = checkBoxSubjectGroup.Checked;
			comboBoxBudgetName.Enabled = checkBoxSubjectGroup.Checked;
		}
	}
}
