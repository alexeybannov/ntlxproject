using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Windows.Forms;
using CI.Debt.DAO;
using CI.Debt.Domain;
using CI.Debt.Impl;
using CI.Debt.Reports;
using CI.Debt.Xml;

namespace CI.Debt.Forms {

	partial class MainForm : Form, IDebtRowView {

		#region IDebtRowView interface

		public DebtType DebtType {
			get { return debtDocProperties.DebtType; }
			set { debtDocProperties.DebtType = value; }
		}

		public int Month {
			get { return debtDocProperties.Month; }
			set { debtDocProperties.Month = value; }
		}

		public int Year {
			get { return debtDocProperties.Year; }
			set { debtDocProperties.Year = value; }
		}

		public void ShowDebtRows(IList<DebtRow> rows) {
			this.rows = new List<DebtRow>(rows);
			rowsDataGrid.Rows.Clear();
			rowsDataGrid.RowCount = this.rows.Count + 1;
		}

		private void debtDocProperties_DebtPropertiesChanged(object sender, EventArgs e) {
			if (DebtRowViewPropertiesChanged != null) {
				DebtRowViewPropertiesChanged(this, e);
			}
		}

		public event EventHandler DebtRowViewPropertiesChanged;

		#endregion

		private ISubjectsPresenter subjectsPresenter;

		private IClassifiersPresenter classifiersPresenter;

		private IDebtRowPresenter debtRowPresenter;

		private WaitForm waitForm;

		public MainForm() {
			InitializeComponent();

			subjectsPresenter = new SubjectsPresenter(new SubjectsForm());
			classifiersPresenter = new ClassifiersPresenter(new ClassifiersForm(ClassifiersFormMode.Browse));
			debtRowPresenter = new DebtRowPresenter(this);
			waitForm = new WaitForm();

			var subjects = new List<Subject>(DebtDAO.GetSubjects());
			var budget = DebtDAO.GetSettings().FilterBudget;
			if (!string.IsNullOrEmpty(budget)) {
				subjects = subjects.FindAll(s => { return string.Compare(budget, s.BudgetName, true) == 0; });
			}
			SubjectColumn.DataSource = subjects;
			SubjectColumn.DisplayMember = "FullName";
			SubjectColumn.ValueMember = "Self";
		}

		private void subjectsToolStripMenuItem_Click(object sender, EventArgs e) {
			subjectsPresenter.ShowSubjects();
		}

		private void classifiersToolStripMenuItem_Click(object sender, EventArgs e) {
			classifiersPresenter.ShowClassifiers();
		}

		private void report1ToolStripMenuItem_Click(object sender, EventArgs e) {
			(new Report1()).ShowReport(DebtType, Month, Year);
		}

		private void exitToolStripMenuItem_Click(object sender, EventArgs e) {
			Close();
		}

		private void MainForm_Shown(object sender, EventArgs e) {
			debtRowPresenter.ShowDebtRows();
			initBooksWorker.RunWorkerAsync();
			waitForm.ShowDialog();
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
			if (row != null) {
				if (rowsDataGrid.Columns[e.ColumnIndex] == AmountColumn) e.Value = row.Amount;
				if (rowsDataGrid.Columns[e.ColumnIndex] == Amount2Column) e.Value = row.Amount2;
				if (rowsDataGrid.Columns[e.ColumnIndex] == SubjectColumn) {
					e.Value = row.Subject;
					if (row.Subject != null && !((IList<Subject>)SubjectColumn.DataSource).Contains(row.Subject)) {
						((IList<Subject>)SubjectColumn.DataSource).Add(row.Subject);
					}
				}
				if (rowsDataGrid.Columns[e.ColumnIndex] == ClassifierColumn) e.Value = row.Classifier;
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
			if (rowsDataGrid.Columns[e.ColumnIndex] == AmountColumn) row.Amount = ParseStringToAmount(e.Value as string);
			if (rowsDataGrid.Columns[e.ColumnIndex] == Amount2Column) row.Amount2 = ParseStringToAmount(e.Value as string);
			if (rowsDataGrid.Columns[e.ColumnIndex] == SubjectColumn) row.Subject = e.Value as Subject;
			if (rowsDataGrid.Columns[e.ColumnIndex] == ClassifierColumn) row.Classifier = e.Value as Classifier;
			DebtDAO.SaveOrUpdateDebtRow(row);
		}

		private void rowsDataGrid_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e) {
			if (0 <= e.Row.Index && e.Row.Index < rows.Count) {
				DebtDAO.RemoveDebtRow(rows[e.Row.Index]);
				rows.RemoveAt(e.Row.Index);
			}
		}

		private DebtRow CreateDebtRow() {
			var defaultSubject = DebtDAO.GetSettings().DefaultSubject;
			if (defaultSubject == null) {
				for (int i = rows.Count - 1; 0 <= i; i--) {
					var row = rows[i];
					if (row.Subject != null) defaultSubject = row.Subject;
				}
			}

			var defaultClassifier = DebtDAO.EmptyClassifier;
			for (int i = rows.Count - 1; 0 <= i; i--) {
				var row = rows[i];
				if (row.Classifier != null) defaultClassifier = row.Classifier;
			}

			return new DebtRow() {
				DebtType = this.DebtType,
				Month = this.Month,
				Year = this.Year,
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

		private void initBooksWorker_DoWork(object sender, DoWorkEventArgs e) {
			try {
				initBooksWorker.ReportProgress(0, "Загрузка справочника кодов бюджетной классификации...");
				DebtDAO.GetClassifiers();
				initBooksWorker.ReportProgress(1, "Загрузка справочника субъектов бюджетного процесса...");
				DebtDAO.GetSubjects();
			}
			catch { }
		}

		private void initBooksWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
			waitForm.CanClose = true;
			waitForm.Close();
		}

		private void initBooksWorker_ProgressChanged(object sender, ProgressChangedEventArgs e) {
			string text = e.UserState as string;
			if (text != null) waitForm.ProgressLabel.Text = text;
		}

		private void buttonExport_Click(object sender, EventArgs e) {
			if (exportFolderBrowser.ShowDialog() != DialogResult.OK) return;

			try {
				string fileName = Path.Combine(exportFolderBrowser.SelectedPath, string.Format("{0}_{1}_{2}.xml", DebtType.ToString(), DebtUtil.GetMonthName(Month), Year));
				XmlDebtRowsSerializer.Serialize(DebtDAO.GetDebtRows(DebtType, Month, Year), fileName);
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
				ImportClassifiers(openFileDialogImport.FileName);

				var rows = new List<DebtRow>(XmlDebtRowsSerializer.Deserialize(openFileDialogImport.FileName));
				if (0 < rows.Count) {
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
					foreach (var row in rows) {
						var existsRow = existsRows.Find(r => { return r.Classifier.Equals(row.Classifier) && r.Subject.Equals(row.Subject); });
						if (existsRow != null) {
							existsRow.Amount = row.Amount;
							existsRow.Amount2 = row.Amount2;
							DebtDAO.SaveOrUpdateDebtRow(existsRow);
						}
						else {
							DebtDAO.SaveOrUpdateDebtRow(row);
						}
					}

					debtDocProperties.SetDebtProperties(rows[0].DebtType, rows[0].Month, rows[0].Year);
				}

				MessageBox.Show(string.Format("Импорт успешно завершен.\r\nФайл импорта: {0}", openFileDialogImport.FileName), Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
			}
			catch (Exception ex) {
				MessageBox.Show(string.Format("Ошибка при импорте задолженности, импорт не выполнен.\r\nТекст ошибки: {0}", ex), Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
			}
		}

		private void ImportClassifiers(string fileName) {
			var xmlClassifiers = XmlDebtRowsSerializer.DeserializeClassifiers(openFileDialogImport.FileName);
			foreach (var xmlClsf in xmlClassifiers) {
				var clsf = DebtDAO.GetClassifier(xmlClsf.ClassifierId);
				if (clsf == null) {
					clsf = new Classifier(xmlClsf.ClassifierId) {
						Code = xmlClsf.ClassifierCode
					};
					DebtDAO.SaveClassifier(clsf);
				}
			}
		}
	}
}