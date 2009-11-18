namespace CI.Debt.Forms {
	partial class DebtCopyForm {
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DebtCopyForm));
			this.labelText = new System.Windows.Forms.Label();
			this.label1 = new System.Windows.Forms.Label();
			this.label2 = new System.Windows.Forms.Label();
			this.buttonOk = new System.Windows.Forms.Button();
			this.buttonCancel = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.DebtPropertiesTo = new CI.Debt.Forms.DebtPropertiesControl();
			this.DebtPropertiesFrom = new CI.Debt.Forms.DebtPropertiesControl();
			this.SuspendLayout();
			// 
			// labelText
			// 
			this.labelText.AutoSize = true;
			this.labelText.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.labelText.Location = new System.Drawing.Point(12, 9);
			this.labelText.Name = "labelText";
			this.labelText.Size = new System.Drawing.Size(335, 13);
			this.labelText.TabIndex = 0;
			this.labelText.Text = "Копировать строки из одной задолженности в другую:";
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label1.ForeColor = System.Drawing.Color.Navy;
			this.label1.Location = new System.Drawing.Point(12, 49);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(53, 13);
			this.label1.TabIndex = 0;
			this.label1.Text = "Откуда:";
			// 
			// label2
			// 
			this.label2.AutoSize = true;
			this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.label2.ForeColor = System.Drawing.Color.DarkRed;
			this.label2.Location = new System.Drawing.Point(12, 103);
			this.label2.Name = "label2";
			this.label2.Size = new System.Drawing.Size(39, 13);
			this.label2.TabIndex = 0;
			this.label2.Text = "Куда:";
			// 
			// buttonOk
			// 
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(311, 149);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(130, 25);
			this.buttonOk.TabIndex = 1;
			this.buttonOk.Text = "&Копировать";
			this.buttonOk.UseVisualStyleBackColor = true;
			// 
			// buttonCancel
			// 
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(447, 149);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(94, 25);
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "&Отмена";
			this.buttonCancel.UseVisualStyleBackColor = true;
			// 
			// groupBox1
			// 
			this.groupBox1.Location = new System.Drawing.Point(12, 130);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Size = new System.Drawing.Size(529, 5);
			this.groupBox1.TabIndex = 0;
			this.groupBox1.TabStop = false;
			// 
			// DebtPropertiesTo
			// 
			this.DebtPropertiesTo.DebtType = CI.Debt.Domain.DebtType.Кредиторская;
			this.DebtPropertiesTo.Location = new System.Drawing.Point(64, 97);
			this.DebtPropertiesTo.Month = 1;
			this.DebtPropertiesTo.Name = "DebtPropertiesTo";
			this.DebtPropertiesTo.Size = new System.Drawing.Size(480, 27);
			this.DebtPropertiesTo.TabIndex = 4;
			this.DebtPropertiesTo.Year = 2009;
			// 
			// DebtPropertiesFrom
			// 
			this.DebtPropertiesFrom.DebtType = CI.Debt.Domain.DebtType.Кредиторская;
			this.DebtPropertiesFrom.Location = new System.Drawing.Point(64, 43);
			this.DebtPropertiesFrom.Month = 1;
			this.DebtPropertiesFrom.Name = "DebtPropertiesFrom";
			this.DebtPropertiesFrom.Size = new System.Drawing.Size(480, 27);
			this.DebtPropertiesFrom.TabIndex = 3;
			this.DebtPropertiesFrom.Year = 2009;
			// 
			// DebtCopyForm
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(550, 181);
			this.Controls.Add(this.groupBox1);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.buttonOk);
			this.Controls.Add(this.label2);
			this.Controls.Add(this.label1);
			this.Controls.Add(this.DebtPropertiesTo);
			this.Controls.Add(this.labelText);
			this.Controls.Add(this.DebtPropertiesFrom);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "DebtCopyForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Копировать задолженность";
			this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.DebtCopyForm_FormClosing);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.Label labelText;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.GroupBox groupBox1;
		public DebtPropertiesControl DebtPropertiesFrom;
		public DebtPropertiesControl DebtPropertiesTo;
	}
}