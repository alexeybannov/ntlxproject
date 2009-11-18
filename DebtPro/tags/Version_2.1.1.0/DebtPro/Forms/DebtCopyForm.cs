using System;
using System.Windows.Forms;
using CI.Debt.Domain;
using CI.Debt.DAO;

namespace CI.Debt.Forms {

	partial class DebtCopyForm : Form {

		public DebtCopyForm(DebtType fromDebtType, int fromMonth, int fromYear) {
			InitializeComponent();

			DebtPropertiesFrom.DebtType = fromDebtType;
			DebtPropertiesFrom.Month = fromMonth;
			DebtPropertiesFrom.Year = fromYear;

			DebtPropertiesTo.DebtType = fromDebtType;
			var date = new DateTime(fromYear, fromMonth, 1).AddMonths(1);
			DebtPropertiesTo.Month = date.Month;
			DebtPropertiesTo.Year = date.Year;
		}

		private void DebtCopyForm_FormClosing(object sender, FormClosingEventArgs e) {
			if (DialogResult == DialogResult.OK) {
				var validateResult = ValidateToDebt();
				e.Cancel = !validateResult;
			}
		}

		private bool ValidateToDebt() {
			if (DebtPropertiesFrom.DebtType == DebtPropertiesTo.DebtType &&
				DebtPropertiesFrom.Month == DebtPropertiesTo.Month &&
				DebtPropertiesFrom.Year == DebtPropertiesTo.Year) {

				var result = MessageBox.Show("Задолженности совпадают. В задолженности будут созданы копии существующих строк.\r\n" +
					"Вы действительно хотите копировать строки в одну и ту же задолженность?", Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
				return result == DialogResult.OK;
			}

			var toDebtRowsCount = DebtDAO.GetDebtRows(DebtPropertiesTo.DebtType, DebtPropertiesTo.Month, DebtPropertiesTo.Year).Count;
			if (toDebtRowsCount != 0) {
				var result = MessageBox.Show("В задолженность, в которую вы хотите скопировать строки, уже имеются строки.\r\n" +
					"Вы действительно хотите копировать строки в непустую задолженность?", Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
				return result == DialogResult.OK;
			}

			return true;
		}
	}
}
