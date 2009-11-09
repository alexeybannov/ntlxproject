using System;
using System.Windows.Forms;
using CI.Debt.Domain;

namespace CI.Debt.Forms {

	/// <summary>
	/// Тип определяет контрол для просмотра и редактирования
	/// свойств строк задолженности.
	/// </summary>
	partial class DebtPropertiesControl : UserControl {

		private DebtType debtType;

		private int month;

		private int year;

		/// <summary>
		/// Тип строки задолженности.
		/// </summary>
		public DebtType DebtType {
			get { return debtType; }
			set {
				if (debtType == value) return;
				debtType = value;
				comboBoxType.SelectedIndex = (int)debtType;
				OnDebtPropertiesChanged();
			}
		}

		/// <summary>
		/// Месяц строки задолженности.
		/// </summary>
		public int Month {
			get { return month; }
			set {
				if (month == value) return;
				month = value;
				comboBoxMonth.SelectedIndex = month - 1;
				OnDebtPropertiesChanged();
			}
		}

		/// <summary>
		/// Год строки задолженности.
		/// </summary>
		public int Year {
			get { return year; }
			set {
				if (year == value) return;
				year = value;
				numericUpDownYear.Value = year;
				OnDebtPropertiesChanged();
			}
		}

		/// <summary>
		/// Создает экземпляр контрола для редактирования свойств строк задолженности.
		/// </summary>
		public DebtPropertiesControl()
			: base() {
			InitializeComponent();

			debtType = DebtType.Кредиторская;
			month = DateTime.Today.Month;
			year = DateTime.Today.Year;
			comboBoxType.SelectedIndex = 0;
			comboBoxMonth.SelectedIndex = 0;
			numericUpDownYear.Value = year;
		}

		/// <summary>
		/// Событие возникает при изменении свойства строки задолженности.
		/// </summary>
		public event DebtPropertiesChangedEventHandler DebtPropertiesChanged;

		public void SetDebtProperties(DebtType type, int month, int year) {
			debtType = type;
			comboBoxType.SelectedIndex = (int)debtType;
			
			this.month = month;
			comboBoxMonth.SelectedIndex = month - 1;
			
			this.year = year;
			numericUpDownYear.Value = year;
			
			OnDebtPropertiesChanged();
		}

		private void OnDebtPropertiesChanged() {
			if (DebtPropertiesChanged != null) {
				DebtPropertiesChanged(this, EventArgs.Empty);
			}
		}

		private void comboBoxType_SelectedIndexChanged(object sender, EventArgs e) {
			if (comboBoxType.SelectedIndex != (int)debtType) {
				debtType = (DebtType)comboBoxType.SelectedIndex;
				OnDebtPropertiesChanged();
			}
		}

		private void comboBoxMonth_SelectedIndexChanged(object sender, EventArgs e) {
			if (comboBoxMonth.SelectedIndex + 1 != month) {
				month = comboBoxMonth.SelectedIndex + 1;
				OnDebtPropertiesChanged();
			}
		}

		private void numericUpDownYear_ValueChanged(object sender, EventArgs e) {
			if ((int)numericUpDownYear.Value != year) {
				year = (int)numericUpDownYear.Value;
				OnDebtPropertiesChanged();
			}
		}
	}

	/// <summary>
	/// Handler события изменения свойства строки задолженности.
	/// </summary>
	delegate void DebtPropertiesChangedEventHandler(object sender, EventArgs e);
}