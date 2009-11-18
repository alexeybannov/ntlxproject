namespace CI.Debt.Forms {
	partial class DebtPropertiesControl {
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent() {
			this.numericUpDownYear = new System.Windows.Forms.NumericUpDown();
			this.label4 = new System.Windows.Forms.Label();
			this.comboBoxMonth = new System.Windows.Forms.ComboBox();
			this.label3 = new System.Windows.Forms.Label();
			this.comboBoxType = new System.Windows.Forms.ComboBox();
			this.label1 = new System.Windows.Forms.Label();
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownYear)).BeginInit();
			this.SuspendLayout();
			// 
			// numericUpDownYear
			// 
			this.numericUpDownYear.Location = new System.Drawing.Point(406, 4);
			this.numericUpDownYear.Maximum = new decimal(new int[] {
            2020,
            0,
            0,
            0});
			this.numericUpDownYear.Minimum = new decimal(new int[] {
            1983,
            0,
            0,
            0});
			this.numericUpDownYear.Name = "numericUpDownYear";
			this.numericUpDownYear.Size = new System.Drawing.Size(66, 20);
			this.numericUpDownYear.TabIndex = 3;
			this.numericUpDownYear.Value = new decimal(new int[] {
            1983,
            0,
            0,
            0});
			this.numericUpDownYear.ValueChanged += new System.EventHandler(this.numericUpDownYear_ValueChanged);
			// 
			// label4
			// 
			this.label4.AutoSize = true;
			this.label4.Location = new System.Drawing.Point(372, 6);
			this.label4.Name = "label4";
			this.label4.Size = new System.Drawing.Size(28, 13);
			this.label4.TabIndex = 0;
			this.label4.Text = "Год:";
			// 
			// comboBoxMonth
			// 
			this.comboBoxMonth.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxMonth.FormattingEnabled = true;
			this.comboBoxMonth.Items.AddRange(new object[] {
            "Январь",
            "Февраль",
            "Март",
            "Апрель",
            "Май",
            "Июнь",
            "Июль",
            "Август",
            "Сентябрь",
            "Октябрь",
            "Ноябрь",
            "Декабрь"});
			this.comboBoxMonth.Location = new System.Drawing.Point(245, 3);
			this.comboBoxMonth.MaxDropDownItems = 12;
			this.comboBoxMonth.Name = "comboBoxMonth";
			this.comboBoxMonth.Size = new System.Drawing.Size(121, 21);
			this.comboBoxMonth.TabIndex = 2;
			this.comboBoxMonth.SelectedIndexChanged += new System.EventHandler(this.comboBoxMonth_SelectedIndexChanged);
			// 
			// label3
			// 
			this.label3.AutoSize = true;
			this.label3.Location = new System.Drawing.Point(196, 6);
			this.label3.Name = "label3";
			this.label3.Size = new System.Drawing.Size(43, 13);
			this.label3.TabIndex = 0;
			this.label3.Text = "Месяц:";
			// 
			// comboBoxType
			// 
			this.comboBoxType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
			this.comboBoxType.FormattingEnabled = true;
			this.comboBoxType.Items.AddRange(new object[] {
            "Кредиторская",
            "Дебиторская"});
			this.comboBoxType.Location = new System.Drawing.Point(38, 3);
			this.comboBoxType.Name = "comboBoxType";
			this.comboBoxType.Size = new System.Drawing.Size(152, 21);
			this.comboBoxType.TabIndex = 1;
			this.comboBoxType.SelectedIndexChanged += new System.EventHandler(this.comboBoxType_SelectedIndexChanged);
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(3, 6);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(29, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Тип:";
			// 
			// DebtPropertiesControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.numericUpDownYear);
			this.Controls.Add(this.label4);
			this.Controls.Add(this.comboBoxMonth);
			this.Controls.Add(this.label3);
			this.Controls.Add(this.comboBoxType);
			this.Controls.Add(this.label1);
			this.Name = "DebtPropertiesControl";
			this.Size = new System.Drawing.Size(480, 27);
			((System.ComponentModel.ISupportInitialize)(this.numericUpDownYear)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.NumericUpDown numericUpDownYear;
		private System.Windows.Forms.Label label4;
		private System.Windows.Forms.ComboBox comboBoxMonth;
		private System.Windows.Forms.Label label3;
		private System.Windows.Forms.ComboBox comboBoxType;
		private System.Windows.Forms.Label label1;
	}
}
