namespace CI.Debt.Reports {
	partial class Report1ParamForm {
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing) {
			if (disposing && (components != null)) {
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Report1ParamForm));
			this.label1 = new System.Windows.Forms.Label();
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.checkBoxSubjectGroup = new System.Windows.Forms.CheckBox();
			this.labelBudgetName = new System.Windows.Forms.Label();
			this.comboBoxBudgetName = new System.Windows.Forms.ComboBox();
			this.DebtProperties = new CI.Debt.Forms.DebtPropertiesControl();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label1.Location = new System.Drawing.Point(12, 9);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(480, 23);
			this.label1.TabIndex = 0;
			this.label1.Text = "Отчет по задолженности";
			this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
			// 
			// buttonOk
			// 
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(254, 160);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(132, 25);
			this.buttonOk.TabIndex = 4;
			this.buttonOk.Text = "&Сформировать";
			this.buttonOk.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(392, 160);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(100, 25);
			this.buttonCancel.TabIndex = 5;
			this.buttonCancel.Text = "&Отмена";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// checkBoxSubjectGroup
			// 
			this.checkBoxSubjectGroup.AutoSize = true;
			this.checkBoxSubjectGroup.Location = new System.Drawing.Point(12, 87);
			this.checkBoxSubjectGroup.Name = "checkBoxSubjectGroup";
			this.checkBoxSubjectGroup.Size = new System.Drawing.Size(169, 17);
			this.checkBoxSubjectGroup.TabIndex = 2;
			this.checkBoxSubjectGroup.Text = "Группировать по субъектам";
			this.checkBoxSubjectGroup.UseVisualStyleBackColor = true;
			this.checkBoxSubjectGroup.CheckedChanged += new System.EventHandler(this.checkBoxSubjectGroup_CheckedChanged);
			// 
			// labelBudgetName
			// 
			this.labelBudgetName.AutoSize = true;
			this.labelBudgetName.Enabled = false;
			this.labelBudgetName.Location = new System.Drawing.Point(9, 113);
			this.labelBudgetName.Name = "labelBudgetName";
			this.labelBudgetName.Size = new System.Drawing.Size(50, 13);
			this.labelBudgetName.TabIndex = 0;
			this.labelBudgetName.Text = "Бюджет:";
			// 
			// comboBoxBudgetName
			// 
			this.comboBoxBudgetName.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxBudgetName.Enabled = false;
			this.comboBoxBudgetName.FormattingEnabled = true;
			this.comboBoxBudgetName.Location = new System.Drawing.Point(68, 110);
			this.comboBoxBudgetName.MaxDropDownItems = 20;
			this.comboBoxBudgetName.Name = "comboBoxBudgetName";
			this.comboBoxBudgetName.Size = new System.Drawing.Size(214, 21);
			this.comboBoxBudgetName.TabIndex = 3;
			// 
			// DebtProperties
			// 
			this.DebtProperties.DebtType = CI.Debt.Domain.DebtType.Кредиторская;
			this.DebtProperties.Location = new System.Drawing.Point(12, 45);
			this.DebtProperties.Month = 1;
			this.DebtProperties.Name = "DebtProperties";
			this.DebtProperties.Size = new System.Drawing.Size(474, 27);
			this.DebtProperties.TabIndex = 1;
			this.DebtProperties.Year = 2008;
			// 
			// Report1ParamForm
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(499, 193);
			this.Controls.Add(this.comboBoxBudgetName);
			this.Controls.Add(this.labelBudgetName);
			this.Controls.Add(this.DebtProperties);
			this.Controls.Add(this.checkBoxSubjectGroup);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.label1);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "Report1ParamForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Параметры отчета";
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.CheckBox checkBoxSubjectGroup;
		public CI.Debt.Forms.DebtPropertiesControl DebtProperties;
		private System.Windows.Forms.Label labelBudgetName;
		public System.Windows.Forms.ComboBox comboBoxBudgetName;
	}
}