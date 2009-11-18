using System.Collections.Generic;
using System.Windows.Forms;
using CI.Debt.DAO;
using CI.Debt.Domain;
using CI.Debt.Utils;
using Microsoft.Reporting.WinForms;

namespace CI.Debt.Reports {

	class Report1 {

		public void ShowReport(DebtType type, int month, int year) {
			var reportParam = new Report1ParamForm(type, month, year);
			if (reportParam.ShowDialog() != DialogResult.OK) return;

			type = reportParam.DebtProperties.DebtType;
			month = reportParam.DebtProperties.Month;
			year = reportParam.DebtProperties.Year;

			string reportCaption = string.Format("Отчет по {0} задолженности за {1} месяц {2} года", (type == DebtType.Кредиторская ? "кредиторской" : "дебиторской"), DebtUtil.GetMonthName(month).ToLower(), year);
			string reportName = (reportParam.GroupBySubjects ? reportName = "CI.Debt.Reports.Report1.rdlc" : "CI.Debt.Reports.Report2.rdlc");

			var reportForm = new ReportForm();
			reportForm.Text = reportCaption;
			reportForm.reportViewer.LocalReport.ReportEmbeddedResource = reportName;
			reportForm.reportViewer.LocalReport.DisplayName = reportCaption;
			reportForm.reportViewer.LocalReport.SetParameters(new[] { new ReportParameter("pCaption", reportCaption) });

			reportForm.Show();


			IList<DebtRow> rows = null;
			if (reportParam.GroupBySubjects && 0 < reportParam.comboBoxBudgetName.SelectedIndex) {
				rows = DebtDAO.GetDebtRows(type, month, year, (string)reportParam.comboBoxBudgetName.SelectedItem);
			}
			else {
				rows = DebtDAO.GetDebtRows(type, month, year);
			}
			var reportRows = new List<Report1Row>();
			foreach (var row in rows) reportRows.Add(new Report1Row(row));

			reportForm.reportViewer.LocalReport.DataSources.Add(new ReportDataSource("Report1Row", reportRows));
			reportForm.reportViewer.RefreshReport();

			reportForm.reportViewer.SetDisplayMode(DisplayMode.PrintLayout);
			reportForm.reportViewer.ZoomMode = ZoomMode.Percent;
			reportForm.reportViewer.ZoomPercent = 100;
		}
	}
}
