namespace CI.Debt.Forms {
	partial class SettingsForm {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
			this.label1 = new System.Windows.Forms.Label();
			this.checkBoxAuto = new System.Windows.Forms.CheckBox();
			this.comboBoxDefSubject = new System.Windows.Forms.ComboBox();
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.checkBoxFilterSubjects = new System.Windows.Forms.CheckBox();
			this.comboBoxBudgets = new System.Windows.Forms.ComboBox();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.SuspendLayout();
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 44);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(126, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Субъект по умолчанию:";
			// 
			// checkBoxAuto
			// 
			this.checkBoxAuto.AutoSize = true;
			this.checkBoxAuto.Location = new System.Drawing.Point(12, 68);
			this.checkBoxAuto.Name = "checkBoxAuto";
			this.checkBoxAuto.Size = new System.Drawing.Size(203, 17);
			this.checkBoxAuto.TabIndex = 1;
			this.checkBoxAuto.Text = "Автоподстановка классификатора";
			this.checkBoxAuto.UseVisualStyleBackColor = true;
			// 
			// comboBoxDefSubject
			// 
			this.comboBoxDefSubject.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxDefSubject.FormattingEnabled = true;
			this.comboBoxDefSubject.Location = new System.Drawing.Point(141, 41);
			this.comboBoxDefSubject.MaxDropDownItems = 25;
			this.comboBoxDefSubject.Name = "comboBoxDefSubject";
			this.comboBoxDefSubject.Size = new System.Drawing.Size(433, 21);
			this.comboBoxDefSubject.TabIndex = 2;
			// 
			// buttonOk
			// 
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(376, 102);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(96, 23);
			this.buttonOk.TabIndex = 3;
			this.buttonOk.Text = "&Да";
			this.buttonOk.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(478, 102);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(96, 23);
			this.buttonCancel.TabIndex = 4;
			this.buttonCancel.Text = "&Отмена";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// checkBoxFilterSubjects
			// 
			this.checkBoxFilterSubjects.AutoSize = true;
			this.checkBoxFilterSubjects.Location = new System.Drawing.Point(12, 16);
			this.checkBoxFilterSubjects.Name = "checkBoxFilterSubjects";
			this.checkBoxFilterSubjects.Size = new System.Drawing.Size(213, 17);
			this.checkBoxFilterSubjects.TabIndex = 5;
			this.checkBoxFilterSubjects.Text = "Фильтровать субъектов по бюджету";
			this.checkBoxFilterSubjects.UseVisualStyleBackColor = true;
			this.checkBoxFilterSubjects.CheckedChanged += new System.EventHandler(this.checkBoxFilterSubjects_CheckedChanged);
			// 
			// comboBoxBudgets
			// 
			this.comboBoxBudgets.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxBudgets.Enabled = false;
			this.comboBoxBudgets.FormattingEnabled = true;
			this.comboBoxBudgets.Location = new System.Drawing.Point(231, 14);
			this.comboBoxBudgets.MaxDropDownItems = 20;
			this.comboBoxBudgets.Name = "comboBoxBudgets";
			this.comboBoxBudgets.Size = new System.Drawing.Size(211, 21);
			this.comboBoxBudgets.TabIndex = 6;
			// 
			// groupBox1
			// 
			this.groupBox1.Location = new System.Drawing.Point(7, 91);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(569, 5);
			this.groupBox1.TabIndex = 7;
			this.groupBox1.TabStop = false;
			// 
			// SettingsForm
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(586, 132);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.comboBoxBudgets);
			this.Controls.Add(this.checkBoxFilterSubjects);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.comboBoxDefSubject);
			this.Controls.Add(this.checkBoxAuto);
			this.Controls.Add(this.label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "SettingsForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Настройки";
			this.Load += new System.EventHandler(this.SettingsForm_Load);
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.SettingsForm_FormClosing);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.CheckBox checkBoxAuto;
		private System.Windows.Forms.ComboBox comboBoxDefSubject;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.CheckBox checkBoxFilterSubjects;
		private System.Windows.Forms.ComboBox comboBoxBudgets;
		private System.Windows.Forms.GroupBox groupBox1;
	}
}