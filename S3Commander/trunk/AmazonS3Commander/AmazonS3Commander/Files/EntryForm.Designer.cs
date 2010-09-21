namespace AmazonS3Commander.Files
{
    partial class EntryForm
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

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(EntryForm));
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkBoxSubfolders = new System.Windows.Forms.CheckBox();
            this.buttonOK = new System.Windows.Forms.Button();
            this.buttonCancel = new System.Windows.Forms.Button();
            this.tabControlProperties = new System.Windows.Forms.TabControl();
            this.tabPageGeneral = new System.Windows.Forms.TabPage();
            this.propertyGridFile = new System.Windows.Forms.PropertyGrid();
            this.tabPageSecurity = new System.Windows.Forms.TabPage();
            this.tabPageHeaders = new System.Windows.Forms.TabPage();
            this.listViewHeaders = new System.Windows.Forms.ListView();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.columnHeader2 = new System.Windows.Forms.ColumnHeader();
            this.panel1.SuspendLayout();
            this.tabControlProperties.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.tabPageHeaders.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.checkBoxSubfolders);
            this.panel1.Controls.Add(this.buttonOK);
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 401);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(411, 41);
            this.panel1.TabIndex = 0;
            // 
            // checkBoxSubfolders
            // 
            this.checkBoxSubfolders.AutoSize = true;
            this.checkBoxSubfolders.Location = new System.Drawing.Point(12, 10);
            this.checkBoxSubfolders.Name = "checkBoxSubfolders";
            this.checkBoxSubfolders.Size = new System.Drawing.Size(173, 17);
            this.checkBoxSubfolders.TabIndex = 3;
            this.checkBoxSubfolders.Text = "Apply for all subfolders and files";
            this.checkBoxSubfolders.UseVisualStyleBackColor = true;
            this.checkBoxSubfolders.Visible = false;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Enabled = false;
            this.buttonOK.Location = new System.Drawing.Point(243, 6);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 1;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(324, 6);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 2;
            this.buttonCancel.Text = "&Cancel";
            this.buttonCancel.UseVisualStyleBackColor = true;
            // 
            // tabControlProperties
            // 
            this.tabControlProperties.Controls.Add(this.tabPageGeneral);
            this.tabControlProperties.Controls.Add(this.tabPageSecurity);
            this.tabControlProperties.Controls.Add(this.tabPageHeaders);
            this.tabControlProperties.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControlProperties.Location = new System.Drawing.Point(0, 0);
            this.tabControlProperties.Name = "tabControlProperties";
            this.tabControlProperties.SelectedIndex = 0;
            this.tabControlProperties.Size = new System.Drawing.Size(411, 401);
            this.tabControlProperties.TabIndex = 0;
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.Controls.Add(this.propertyGridFile);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGeneral.Size = new System.Drawing.Size(403, 375);
            this.tabPageGeneral.TabIndex = 0;
            this.tabPageGeneral.Text = "General";
            this.tabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // propertyGridFile
            // 
            this.propertyGridFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridFile.Location = new System.Drawing.Point(3, 3);
            this.propertyGridFile.Name = "propertyGridFile";
            this.propertyGridFile.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.propertyGridFile.Size = new System.Drawing.Size(397, 369);
            this.propertyGridFile.TabIndex = 0;
            this.propertyGridFile.ToolbarVisible = false;
            // 
            // tabPageSecurity
            // 
            this.tabPageSecurity.Location = new System.Drawing.Point(4, 22);
            this.tabPageSecurity.Name = "tabPageSecurity";
            this.tabPageSecurity.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSecurity.Size = new System.Drawing.Size(403, 375);
            this.tabPageSecurity.TabIndex = 1;
            this.tabPageSecurity.Text = "Security";
            this.tabPageSecurity.UseVisualStyleBackColor = true;
            // 
            // tabPageHeaders
            // 
            this.tabPageHeaders.Controls.Add(this.listViewHeaders);
            this.tabPageHeaders.Location = new System.Drawing.Point(4, 22);
            this.tabPageHeaders.Name = "tabPageHeaders";
            this.tabPageHeaders.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageHeaders.Size = new System.Drawing.Size(403, 375);
            this.tabPageHeaders.TabIndex = 2;
            this.tabPageHeaders.Text = "HTTP Headers";
            this.tabPageHeaders.UseVisualStyleBackColor = true;
            // 
            // listViewHeaders
            // 
            this.listViewHeaders.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeader1,
            this.columnHeader2});
            this.listViewHeaders.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listViewHeaders.FullRowSelect = true;
            this.listViewHeaders.HideSelection = false;
            this.listViewHeaders.Location = new System.Drawing.Point(3, 3);
            this.listViewHeaders.MultiSelect = false;
            this.listViewHeaders.Name = "listViewHeaders";
            this.listViewHeaders.ShowGroups = false;
            this.listViewHeaders.Size = new System.Drawing.Size(397, 369);
            this.listViewHeaders.TabIndex = 0;
            this.listViewHeaders.UseCompatibleStateImageBehavior = false;
            this.listViewHeaders.View = System.Windows.Forms.View.Details;
            // 
            // columnHeader1
            // 
            this.columnHeader1.Text = "Header";
            this.columnHeader1.Width = 150;
            // 
            // columnHeader2
            // 
            this.columnHeader2.Text = "Value";
            this.columnHeader2.Width = 230;
            // 
            // EntryForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(411, 442);
            this.Controls.Add(this.tabControlProperties);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "EntryForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "EntryForm";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabControlProperties.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            this.tabPageHeaders.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.TabControl tabControlProperties;
        private System.Windows.Forms.TabPage tabPageGeneral;
        private System.Windows.Forms.TabPage tabPageSecurity;
        private System.Windows.Forms.Button buttonOK;
        private System.Windows.Forms.Button buttonCancel;
        private System.Windows.Forms.PropertyGrid propertyGridFile;
        private System.Windows.Forms.TabPage tabPageHeaders;
        private System.Windows.Forms.ListView listViewHeaders;
        private System.Windows.Forms.ColumnHeader columnHeader1;
        private System.Windows.Forms.ColumnHeader columnHeader2;
        private System.Windows.Forms.CheckBox checkBoxSubfolders;
    }
}