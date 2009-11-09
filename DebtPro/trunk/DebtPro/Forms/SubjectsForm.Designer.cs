namespace CI.Debt.Forms {
    partial class SubjectsForm {
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SubjectsForm));
			this.subjectsView = new System.Windows.Forms.DataGridView();
			this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SubjectName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Code = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.BudgetName = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.buttonClose = new System.Windows.Forms.Button();
			((System.ComponentModel.ISupportInitialize)(this.subjectsView)).BeginInit();
			this.SuspendLayout();
			// 
			// subjectsView
			// 
			this.subjectsView.AllowUserToAddRows = false;
			this.subjectsView.AllowUserToDeleteRows = false;
			this.subjectsView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.subjectsView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.subjectsView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.SubjectName,
            this.Code,
            this.BudgetName});
			this.subjectsView.Location = new System.Drawing.Point(13, 13);
			this.subjectsView.Name = "subjectsView";
			this.subjectsView.ReadOnly = true;
			this.subjectsView.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
			this.subjectsView.Size = new System.Drawing.Size(816, 465);
			this.subjectsView.TabIndex = 1;
			// 
			// Id
			// 
			this.Id.DataPropertyName = "Id";
			this.Id.HeaderText = "Id";
			this.Id.Name = "Id";
			this.Id.ReadOnly = true;
			this.Id.Visible = false;
			// 
			// SubjectName
			// 
			this.SubjectName.DataPropertyName = "Name";
			this.SubjectName.HeaderText = "Наименование субъекта";
			this.SubjectName.Name = "SubjectName";
			this.SubjectName.ReadOnly = true;
			this.SubjectName.Width = 500;
			// 
			// Code
			// 
			this.Code.DataPropertyName = "Code";
			dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
			this.Code.DefaultCellStyle = dataGridViewCellStyle1;
			this.Code.HeaderText = "Код";
			this.Code.Name = "Code";
			this.Code.ReadOnly = true;
			// 
			// BudgetName
			// 
			this.BudgetName.DataPropertyName = "BudgetName";
			this.BudgetName.HeaderText = "Бюджет";
			this.BudgetName.Name = "BudgetName";
			this.BudgetName.ReadOnly = true;
			this.BudgetName.Width = 150;
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonClose.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonClose.Location = new System.Drawing.Point(753, 484);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(75, 23);
			this.buttonClose.TabIndex = 2;
			this.buttonClose.Text = "Закрыть";
			this.buttonClose.UseVisualStyleBackColor = true;
			// 
			// SubjectsForm
			// 
			this.AcceptButton = this.buttonClose;
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonClose;
			this.ClientSize = new System.Drawing.Size(841, 519);
			this.Controls.Add(this.buttonClose);
			this.Controls.Add(this.subjectsView);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.Name = "SubjectsForm";
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Учреждения";
			((System.ComponentModel.ISupportInitialize)(this.subjectsView)).EndInit();
			this.ResumeLayout(false);

        }

        #endregion

		private System.Windows.Forms.DataGridView subjectsView;
		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.DataGridViewTextBoxColumn Id;
		private System.Windows.Forms.DataGridViewTextBoxColumn SubjectName;
		private System.Windows.Forms.DataGridViewTextBoxColumn Code;
		private System.Windows.Forms.DataGridViewTextBoxColumn BudgetName;
    }
}