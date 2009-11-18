using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using CI.Debt.Domain;
using CI.Debt.DAO;

namespace CI.Debt.Forms {

	partial class SettingsForm : Form {

		private DebtSettings settings;

		public SettingsForm() {
			InitializeComponent();
		}

		private void SettingsForm_Load(object sender, EventArgs e) {
			comboBoxDefSubject.DataSource = DebtDAO.GetSubjects();
			settings = DebtDAO.GetSettings();
			if (settings.DefaultSubject != null) comboBoxDefSubject.SelectedItem = settings.DefaultSubject;

			checkBoxAuto.Checked = settings.IsAutoPasteClassifier;

			comboBoxBudgets.DataSource = DebtDAO.GetBudgets();
			if (!string.IsNullOrEmpty(settings.FilterBudget)) {
				checkBoxFilterSubjects.Checked = true;
				if (comboBoxBudgets.Items.Contains(settings.FilterBudget)) comboBoxBudgets.SelectedItem = settings.FilterBudget;

			}
			else {
				checkBoxFilterSubjects.Checked = false;
			}
		}

		private void SettingsForm_FormClosing(object sender, FormClosingEventArgs e) {
			if (DialogResult == DialogResult.OK) {
				settings.DefaultSubject = comboBoxDefSubject.SelectedItem as Subject;
				settings.IsAutoPasteClassifier = checkBoxAuto.Checked;
				if (checkBoxFilterSubjects.Checked) settings.FilterBudget = comboBoxBudgets.SelectedItem as string;
				else settings.FilterBudget = null;
				DebtDAO.SaveSettings(settings);
			}
		}

		private void checkBoxFilterSubjects_CheckedChanged(object sender, EventArgs e) {
			comboBoxBudgets.Enabled = checkBoxFilterSubjects.Checked;
		}
	}
}
