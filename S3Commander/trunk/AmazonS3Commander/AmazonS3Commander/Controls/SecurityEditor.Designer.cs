namespace AmazonS3Commander.Controls
{
    partial class SecurityEditor
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonRemove = new System.Windows.Forms.Button();
            this.buttonAdd = new System.Windows.Forms.Button();
            this.dataGridViewACL = new System.Windows.Forms.DataGridView();
            this.dataGridViewImageColumn1 = new System.Windows.Forms.DataGridViewImageColumn();
            this.Column1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Column2 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column3 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column4 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column5 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.Column6 = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewACL)).BeginInit();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonRemove);
            this.panel1.Controls.Add(this.buttonAdd);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 297);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(465, 35);
            this.panel1.TabIndex = 1;
            // 
            // buttonRemove
            // 
            this.buttonRemove.Location = new System.Drawing.Point(84, 6);
            this.buttonRemove.Name = "buttonRemove";
            this.buttonRemove.Size = new System.Drawing.Size(75, 23);
            this.buttonRemove.TabIndex = 1;
            this.buttonRemove.Text = "&Remove";
            this.buttonRemove.UseVisualStyleBackColor = true;
            // 
            // buttonAdd
            // 
            this.buttonAdd.Location = new System.Drawing.Point(3, 6);
            this.buttonAdd.Name = "buttonAdd";
            this.buttonAdd.Size = new System.Drawing.Size(75, 23);
            this.buttonAdd.TabIndex = 1;
            this.buttonAdd.Text = "&Add";
            this.buttonAdd.UseVisualStyleBackColor = true;
            // 
            // dataGridViewACL
            // 
            this.dataGridViewACL.AllowUserToAddRows = false;
            this.dataGridViewACL.AllowUserToDeleteRows = false;
            this.dataGridViewACL.AllowUserToResizeRows = false;
            this.dataGridViewACL.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewACL.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.dataGridViewACL.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.dataGridViewACL.ClipboardCopyMode = System.Windows.Forms.DataGridViewClipboardCopyMode.EnableWithoutHeaderText;
            this.dataGridViewACL.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewACL.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.dataGridViewImageColumn1,
            this.Column1,
            this.Column2,
            this.Column3,
            this.Column4,
            this.Column5,
            this.Column6});
            this.dataGridViewACL.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewACL.Location = new System.Drawing.Point(0, 0);
            this.dataGridViewACL.MultiSelect = false;
            this.dataGridViewACL.Name = "dataGridViewACL";
            this.dataGridViewACL.RowHeadersVisible = false;
            this.dataGridViewACL.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewACL.ShowEditingIcon = false;
            this.dataGridViewACL.Size = new System.Drawing.Size(465, 297);
            this.dataGridViewACL.TabIndex = 1;
            this.dataGridViewACL.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridViewACL_CellValueChanged);
            // 
            // dataGridViewImageColumn1
            // 
            this.dataGridViewImageColumn1.HeaderText = "";
            this.dataGridViewImageColumn1.Name = "dataGridViewImageColumn1";
            this.dataGridViewImageColumn1.ReadOnly = true;
            this.dataGridViewImageColumn1.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.dataGridViewImageColumn1.Width = 16;
            // 
            // Column1
            // 
            this.Column1.HeaderText = "Username";
            this.Column1.Name = "Column1";
            this.Column1.ReadOnly = true;
            this.Column1.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.Column1.Width = 120;
            // 
            // Column2
            // 
            this.Column2.HeaderText = "Full Control";
            this.Column2.Name = "Column2";
            this.Column2.Width = 65;
            // 
            // Column3
            // 
            this.Column3.HeaderText = "Read";
            this.Column3.Name = "Column3";
            this.Column3.ReadOnly = true;
            this.Column3.Width = 65;
            // 
            // Column4
            // 
            this.Column4.HeaderText = "Write";
            this.Column4.Name = "Column4";
            this.Column4.Width = 65;
            // 
            // Column5
            // 
            this.Column5.HeaderText = "Read ACL";
            this.Column5.Name = "Column5";
            this.Column5.Width = 65;
            // 
            // Column6
            // 
            this.Column6.HeaderText = "Write ACL";
            this.Column6.Name = "Column6";
            this.Column6.Width = 65;
            // 
            // SecurityEditor
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.dataGridViewACL);
            this.Controls.Add(this.panel1);
            this.Name = "SecurityEditor";
            this.Size = new System.Drawing.Size(465, 332);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewACL)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonRemove;
        private System.Windows.Forms.Button buttonAdd;
        private System.Windows.Forms.DataGridView dataGridViewACL;
        private System.Windows.Forms.DataGridViewImageColumn dataGridViewImageColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn Column1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column2;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column3;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column4;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column5;
        private System.Windows.Forms.DataGridViewCheckBoxColumn Column6;

    }
}
