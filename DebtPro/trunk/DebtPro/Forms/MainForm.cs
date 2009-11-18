using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using CI.Debt.DAO;
using CI.Debt.Domain;
using CI.Debt.Utils;
using CI.Debt.Reports;
using CI.Debt.Xml;
using System.Reflection;
using System.Diagnostics;

namespace CI.Debt.Forms {

	partial class MainForm : Form {

		private WaitForm waitForm;

		private bool initialized = false;

		private List<Subject> subjects;

		private bool subjectsFiltered = false;

		public MainForm() {
			InitializeComponent();
			waitForm = new WaitForm();
		}

		private void MainForm_Shown(object sender, EventArgs e) {
			initWorker.RunWorkerAsync();
			waitForm.ShowDialog();
		}

		private void initBooksWorker_DoWork(object sender, DoWorkEventArgs e) {
			try {
				DebtDAO.Initialize();
			}
			catch { }
		}

		private void initBooksWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			waitForm.CanClose = true;
			waitForm.Close();
			waitForm.Dispose();
			waitForm = null;

			subjects = DebtDAO.GetSubjects();
			var budget = DebtDAO.GetSettings().FilterBudget;
			if (!string.IsNullOrEmpty(budget)) {
				subjects = subjects.FindAll(s => { return string.Compare(budget, s.BudgetName, true) == 0; });
				subjectsFiltered = true;
			}
			SubjectColumn.DataSource = subjects;
			SubjectColumn.DisplayMember = "FullName";
			SubjectColumn.ValueMember = "Self";

			initialized = true;
			ShowDebtRows();
		}

		private void ShowDebtRows() {
			rows = new List<DebtRow>(
				DebtDAO.GetDebtRows(debtDocProperties.DebtType, debtDocProperties.Month, debtDocProperties.Year)
			);
			rowsDataGrid.Rows.Clear();
			rowsDataGrid.RowCount = this.rows.Count + 1;
		}

		private void debtDocProperties_DebtPropertiesChanged(object sender, EventArgs e) {
			ShowDebtRows();
		}


		private void subjectsToolStripMenuItem_Click(object sender, EventArgs e) {
			new SubjectsForm().ShowDialog(this);
		}

		private void classifiersToolStripMenuItem_Click(object sender, EventArgs e) {
			new ClassifiersForm().ShowDialog(this);
		}

		private void report1ToolStripMenuItem_Click(object sender, EventArgs e) {
			(new Report1()).ShowReport(debtDocProperties.DebtType, debtDocProperties.Month, debtDocProperties.Year);
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
			Close();
		}

		#region Virtual mode of DataGridView implementing

		private List<DebtRow> rows = new List<DebtRow>();

		private DebtRow newRow;

		private void rowsDataGrid_NewRowNeeded(object sender, DataGridViewRowEventArgs e) {
			newRow = CreateDebtRow();
		}

		private void rowsDataGrid_CellValueNeeded(object sender, DataGridViewCellValueEventArgs e) {
			if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

			var row = e.RowIndex < rows.Count ? rows[e.RowIndex] : newRow;
			if (row == null) return;

			if (e.ColumnIndex == 0) e.Value = row.Classifier;
			if (e.ColumnIndex == 1) e.Value = row.Amount;
			if (e.ColumnIndex == 2) e.Value = row.Amount2;
			if (e.ColumnIndex == 3) {
				e.Value = row.Subject;
				if (subjectsFiltered && row.Subject != null && !subjects.Contains(row.Subject)) {
					subjects.Add(row.Subject);
				}
			}
		}

		private void rowsDataGrid_CellValuePushed(object sender, DataGridViewCellValueEventArgs e) {
			if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

			DebtRow row = null;
			if (e.RowIndex < rows.Count) {
				row = rows[e.RowIndex];
			}
			else {
				row = newRow;
				rows.Add(row);
			}
			if (e.ColumnIndex == 0) row.Classifier = e.Value as Classifier;
			if (e.ColumnIndex == 1) row.Amount = ParseStringToAmount(e.Value as string);
			if (e.ColumnIndex == 2) row.Amount2 = ParseStringToAmount(e.Value as string);
			if (e.ColumnIndex == 3) row.Subject = e.Value as Subject;
			DebtDAO.SaveOrUpdateDebtRow(row);
		}

		private void rowsDataGrid_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) {
			if (0 <= e.Row.Index && e.Row.Index < rows.Count) {
				DebtDAO.RemoveDebtRow(rows[e.Row.Index]);
				rows.RemoveAt(e.Row.Index);
			}
		}

		private DebtRow CreateDebtRow() {
			if (!initialized) return null;

			var defaultSubject = DebtDAO.GetSettings().DefaultSubject;
			if (defaultSubject == null) {
				for (int i = rows.Count - 1; 0 <= i; i--) {
					if (rows[i].Subject != null) {
						defaultSubject = rows[i].Subject;
						break;
					}
				}
			}

			var defaultClassifier = Classifier.Empty;
			for (int i = rows.Count - 1; 0 <= i; i--) {
				if (rows[i].Classifier != null) {
					defaultClassifier = rows[i].Classifier;
					break;
				}
			}

			return new DebtRow() {
				DebtType = debtDocProperties.DebtType,
				Month = debtDocProperties.Month,
				Year = debtDocProperties.Year,
				Subject = defaultSubject,
				Classifier = defaultClassifier,
			};
		}

		private double ParseStringToAmount(string amountStr) {
			double amount = 0.0;
			if (string.IsNullOrEmpty(amountStr)) return amount;
			var separator = double.TryParse("0.0", out amount) ? "." : ",";
			amountStr = amountStr.Replace(".", separator).Replace(",", separator);
			double.TryParse(amountStr, out amount);
			return amount;
		}

		#endregion

		private void aboutToolStripMenuItem_Click(object sender, EventArgs e) {
			(new AboutBox()).ShowDialog();
		}

		private void buttonExport_Click(object sender, EventArgs e) {
			if (exportFolderBrowser.ShowDialog() != DialogResult.OK) return;

			try {
				string fileName = Path.Combine(
					exportFolderBrowser.SelectedPath,
					string.Format("{0}_{1}_{2}.xml", debtDocProperties.DebtType.ToString(), DebtUtil.GetMonthName(debtDocProperties.Month), debtDocProperties.Year)
				);
				XmlDebtRowsSerializer.Serialize(DebtDAO.GetDebtRows(debtDocProperties.DebtType, debtDocProperties.Month, debtDocProperties.Year), fileName);
				MessageBox.Show(string.Format("Экспорт успешно завершен.\r\nФайл экспорта: {0}", fileName), Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex) {
				MessageBox.Show("При экспорте возникло програмное исключение. Экспорт не выполнен.\r\nТекст ошибки: " + ex.ToString(), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void settingsToolStripMenuItem_Click(object sender, EventArgs e) {
			var settingsForm = new SettingsForm();
			settingsForm.ShowDialog();
		}

		private void toolStripMenuItemCopy_Click(object sender, EventArgs e) {
			var copyForm = new DebtCopyForm(debtDocProperties.DebtType, debtDocProperties.Month, debtDocProperties.Year);
			if (copyForm.ShowDialog(this) != DialogResult.OK) return;

			DebtDAO.CopyDebtRows(
				copyForm.DebtPropertiesFrom.DebtType,
				copyForm.DebtPropertiesFrom.Month,
				copyForm.DebtPropertiesFrom.Year,
				copyForm.DebtPropertiesTo.DebtType,
				copyForm.DebtPropertiesTo.Month,
				copyForm.DebtPropertiesTo.Year);

			MessageBox.Show("Копирование строк задолженности успешно завершено.", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);

			debtDocProperties.SetDebtProperties(copyForm.DebtPropertiesTo.DebtType, copyForm.DebtPropertiesTo.Month, copyForm.DebtPropertiesTo.Year);
		}

		private void toolStripMenuItemImport_Click(object sender, EventArgs e) {
			if (openFileDialogImport.ShowDialog() != DialogResult.OK) return;

			try {
				var rows = new List<DebtRow>(XmlDebtRowsSerializer.Deserialize(openFileDialogImport.FileName));
				if (rows.Count == 0) return;

				var existsRows = new List<DebtRow>(DebtDAO.GetDebtRows(rows[0].DebtType, rows[0].Month, rows[0].Year));
				if (0 < existsRows.Count) {
					var answer = MessageBox.Show("Уже существуют строки задолженности такого же типа, месяца и года, как и в файле импорта.\r\nПродолжить импорт?\r\nДа - продолжить импорт, Отмена - прервать импорт.", Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
					if (answer == DialogResult.Cancel) return;
				}
				foreach (var row in rows) {
					if (existsRows.Exists(r => { return r.Classifier.Equals(row.Classifier) && r.Subject.Equals(row.Subject); })) {
						var answer = MessageBox.Show("Уже существуют строки задолженности с такими же классификатором и субъектом, как и в файле импорта.\r\nПродолжить импорт и перезаписать существующие строки?\r\nДа - продолжить импорт, Отмена - прервать импорт.", Text, MessageBoxButtons.OKCancel, MessageBoxIcon.Question);
						if (answer == DialogResult.Cancel) return;
						break;
					}
				}

				var rowsToSave = new List<DebtRow>();
				foreach (var row in rows) {
					var existsRow = existsRows.Find(r => { return r.Classifier.Equals(row.Classifier) && r.Subject.Equals(row.Subject); });
					if (existsRow != null) {
						existsRow.Amount = row.Amount;
						existsRow.Amount2 = row.Amount2;
						rowsToSave.Add(existsRow);
					}
					else {
						rowsToSave.Add(row);
					}
				}
				DebtDAO.SaveOrUpdateDebtRows(rowsToSave);

				debtDocProperties.SetDebtProperties(rows[0].DebtType, rows[0].Month, rows[0].Year);

				MessageBox.Show(string.Format("Импорт успешно завершен.\r\nФайл импорта: {0}", openFileDialogImport.FileName), Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex) {
				MessageBox.Show(string.Format("Ошибка при импорте задолженности, импорт не выполнен.\r\nТекст ошибки: {0}", ex), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void changeHistoryToolStripMenuItem_Click(object sender, EventArgs e) {
			try {
				byte[] historyBuffer = null;
				using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("CI.Debt.ChangeHistory.txt")) {
					var memStream = new MemoryStream();
					var buffer = new byte[1024];
					int readBytes = 0;
					while ((readBytes = stream.Read(buffer, 0, buffer.Length)) != 0) {
						memStream.Write(buffer, 0, readBytes);
					}
					historyBuffer = memStream.GetBuffer();
				}
				var historyFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "History.txt");
				if (!File.Exists(historyFile) || new FileInfo(historyFile).Length != historyBuffer.Length) {
					File.WriteAllBytes(historyFile, historyBuffer);
				}
				Process.Start("notepad.exe", historyFile);
			}
			catch { }
		}
	}
}