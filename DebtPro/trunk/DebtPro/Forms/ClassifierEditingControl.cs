using System;
using System.Reflection;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using CI.Debt.Domain;
using CI.Debt.DAO;
using CI.Debt.Impl;

// Подробнее о DataGridView см. http://www.rsdn.ru/article/dotnet/datagridfaq.xml?print
// http://www.rsdn.ru/article/dotnet/DataGridView20.xml?print
// http://www.rsdn.ru/article/dotnet/DataGridView20part2.xml?print

namespace CI.Debt.Forms {

	/// <summary>
	/// Тип определет контрол редактирования в столбце классфикатора.
	/// </summary>
	public partial class ClassifierEditingControl : UserControl, IDataGridViewEditingControl {

		private IClassifiersPresenter classifiersPresenter;

		private MaskedTextBox maskedTextBox;

		private Panel panel1;

		private Button buttonSelect;

		private FieldInfo caretField;

		/// <inheritdoc/>
		public ClassifierEditingControl() {
			InitializeComponent();
			classifiersPresenter = new ClassifiersPresenter(new ClassifiersForm(ClassifiersFormMode.Select));
			classifiersPresenter.ClassifierSelected += ClassifierSelected;
			caretField = typeof(MaskedTextBox).GetField("caretTestPos", BindingFlags.NonPublic | BindingFlags.Instance);
		}

		protected override void Dispose(bool disposing) {
			if (disposing) classifiersPresenter.ClassifierSelected -= ClassifierSelected;
			base.Dispose(disposing);
		}

		private void InitializeComponent() {
			this.maskedTextBox = new System.Windows.Forms.MaskedTextBox();
			this.buttonSelect = new System.Windows.Forms.Button();
			this.panel1 = new System.Windows.Forms.Panel();
			this.panel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// maskedTextBox
			// 
			this.maskedTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.maskedTextBox.HideSelection = false;
			this.maskedTextBox.InsertKeyMode = System.Windows.Forms.InsertKeyMode.Overwrite;
			this.maskedTextBox.Location = new System.Drawing.Point(0, 0);
			this.maskedTextBox.Mask = "000\\.00 00\\.000 00 00\\.000\\.000\\.000\\:000";
			this.maskedTextBox.Name = "maskedTextBox";
			this.maskedTextBox.Size = new System.Drawing.Size(290, 20);
			this.maskedTextBox.TabIndex = 1;
			this.maskedTextBox.TextMaskFormat = System.Windows.Forms.MaskFormat.IncludePromptAndLiterals;
			this.maskedTextBox.TextChanged += new System.EventHandler(this.maskedTextBox_TextChanged);
			// 
			// buttonSelect
			// 
			this.buttonSelect.BackColor = System.Drawing.SystemColors.ButtonFace;
			this.buttonSelect.Dock = System.Windows.Forms.DockStyle.Right;
			this.buttonSelect.Location = new System.Drawing.Point(290, 0);
			this.buttonSelect.Name = "buttonSelect";
			this.buttonSelect.Size = new System.Drawing.Size(25, 29);
			this.buttonSelect.TabIndex = 2;
			this.buttonSelect.Text = "...";
			this.buttonSelect.UseVisualStyleBackColor = false;
			this.buttonSelect.Click += new System.EventHandler(this.buttonSelect_Click);
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.maskedTextBox);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.panel1.Location = new System.Drawing.Point(0, 0);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(290, 29);
			this.panel1.TabIndex = 1;
			// 
			// ClassifierEditingControl
			// 
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.buttonSelect);
			this.Name = "ClassifierEditingControl";
			this.Size = new System.Drawing.Size(315, 29);
			this.panel1.ResumeLayout(false);
			this.panel1.PerformLayout();
			this.ResumeLayout(false);

		}

		private void buttonSelect_Click(object sender, EventArgs e) {
			var clsf = EditingControlDataGridView.CurrentCell.Value as Classifier;
			classifiersPresenter.SelectedClassifier = clsf ?? Classifier.Empty;
			classifiersPresenter.ShowClassifiers();
		}

		private void ClassifierSelected(object sender, EventArgs e) {
			Classifier clsf = ((IClassifiersPresenter)sender).SelectedClassifier;
			if (clsf != null) {
				EditingControlDataGridView.CurrentCell.Value = clsf;
				EditingControlFormattedValue = clsf.MaskedCode;
			}
		}

		/// <inheritdoc/>
		public object EditingControlFormattedValue {
			get { return Text; }
			set { Text = value != null ? value.ToString() : string.Empty; }
		}

		/// <inheritdoc/>
		public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context) {
			return EditingControlFormattedValue;
		}

		/// <inheritdoc/>
		public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle) {
			Font = dataGridViewCellStyle.Font;
		}

		/// <inheritdoc/>
		public int EditingControlRowIndex {
			get;
			set;
		}

		/// <inheritdoc/>
		public bool EditingControlWantsInputKey(Keys key, bool dataGridViewWantsInputKey) {
			switch (key & Keys.KeyCode) {
				case Keys.Left:
				case Keys.Up:
				case Keys.Down:
				case Keys.Right:
				case Keys.Home:
				case Keys.End:
				case Keys.PageDown:
				case Keys.PageUp:
					return true;
				default:
					return false;
			}
		}

		/// <inheritdoc/>
		public void PrepareEditingControlForEdit(bool selectAll) {
			maskedTextBox.SelectionStart = 0;
			if (EditingControlDataGridView != null) {
				var clsf = EditingControlDataGridView.CurrentCell.Value as Classifier;
				if (!Classifier.Empty.Equals(clsf)) maskedTextBox.SelectionStart = maskedTextBox.TextLength - 7;
			}
		}

		/// <inheritdoc/>
		public bool RepositionEditingControlOnValueChange {
			get { return false; }
		}

		/// <inheritdoc/>
		public DataGridView EditingControlDataGridView {
			get;
			set;
		}

		/// <inheritdoc/>
		public bool EditingControlValueChanged {
			get;
			set;
		}

		/// <inheritdoc/>
		public Cursor EditingPanelCursor {
			get { return base.Cursor; }
		}

		private void maskedTextBox_TextChanged(object sender, EventArgs e) {
			if (EditingControlDataGridView != null) EditingControlDataGridView.NotifyCurrentCellDirty(true);
			EditingControlValueChanged = true;

			if (!DebtDAO.GetSettings().IsAutoPasteClassifier) return;
			//автоподстановка классификатора
			int position = (int)caretField.GetValue(maskedTextBox) + 1;
			string code = DebtUtil.RemoveMaskSymbols(Text.Substring(0, position));
			if (string.IsNullOrEmpty(code) || code.Length == Classifier.CodeLenght) return;

			var clsf = DebtDAO.FindNearestClassifier(code);
			string newCode = null;
			if (clsf != null) newCode = clsf.MaskedCode;
			else newCode = code + new string('0', Classifier.CodeLenght).Substring(0, Classifier.CodeLenght);
			position -= 1;
			Text = newCode;
			if (0 <= position && position < maskedTextBox.MaskedTextProvider.Length) caretField.SetValue(maskedTextBox, position);
		}

		public override string Text {
			get { return maskedTextBox.Text; }
			set { maskedTextBox.Text = value; }
		}
	}
}