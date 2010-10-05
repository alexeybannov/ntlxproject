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
            this.securityEditor1 = new AmazonS3Commander.Controls.SecurityEditor();
            this.tabPageHeaders = new System.Windows.Forms.TabPage();
            this.httpHeadersEditor = new AmazonS3Commander.Controls.HttpHeadersEditor();
            this.panel1.SuspendLayout();
            this.tabControlProperties.SuspendLayout();
            this.tabPageGeneral.SuspendLayout();
            this.tabPageSecurity.SuspendLayout();
            this.tabPageHeaders.SuspendLayout();
            this.SuspendLayout();
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.checkBoxSubfolders);
            this.panel1.Controls.Add(this.buttonOK);
            this.panel1.Controls.Add(this.buttonCancel);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 393);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(479, 41);
            this.panel1.TabIndex = 0;
            // 
            // checkBoxSubfolders
            // 
            this.checkBoxSubfolders.AutoSize = true;
            this.checkBoxSubfolders.Location = new System.Drawing.Point(12, 10);
            this.checkBoxSubfolders.Name = "checkBoxSubfolders";
            this.checkBoxSubfolders.Size = new System.Drawing.Size(173, 17);
            this.checkBoxSubfolders.TabIndex = 2;
            this.checkBoxSubfolders.Text = "Apply for all subfolders and files";
            this.checkBoxSubfolders.UseVisualStyleBackColor = true;
            this.checkBoxSubfolders.Visible = false;
            // 
            // buttonOK
            // 
            this.buttonOK.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonOK.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.buttonOK.Enabled = false;
            this.buttonOK.Location = new System.Drawing.Point(311, 6);
            this.buttonOK.Name = "buttonOK";
            this.buttonOK.Size = new System.Drawing.Size(75, 23);
            this.buttonOK.TabIndex = 3;
            this.buttonOK.Text = "&OK";
            this.buttonOK.UseVisualStyleBackColor = true;
            // 
            // buttonCancel
            // 
            this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.buttonCancel.Location = new System.Drawing.Point(392, 6);
            this.buttonCancel.Name = "buttonCancel";
            this.buttonCancel.Size = new System.Drawing.Size(75, 23);
            this.buttonCancel.TabIndex = 4;
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
            this.tabControlProperties.Size = new System.Drawing.Size(479, 393);
            this.tabControlProperties.TabIndex = 1;
            // 
            // tabPageGeneral
            // 
            this.tabPageGeneral.Controls.Add(this.propertyGridFile);
            this.tabPageGeneral.Location = new System.Drawing.Point(4, 22);
            this.tabPageGeneral.Name = "tabPageGeneral";
            this.tabPageGeneral.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageGeneral.Size = new System.Drawing.Size(471, 367);
            this.tabPageGeneral.TabIndex = 1;
            this.tabPageGeneral.Text = "General";
            this.tabPageGeneral.UseVisualStyleBackColor = true;
            // 
            // propertyGridFile
            // 
            this.propertyGridFile.Dock = System.Windows.Forms.DockStyle.Fill;
            this.propertyGridFile.Location = new System.Drawing.Point(3, 3);
            this.propertyGridFile.Name = "propertyGridFile";
            this.propertyGridFile.PropertySort = System.Windows.Forms.PropertySort.Alphabetical;
            this.propertyGridFile.Size = new System.Drawing.Size(465, 361);
            this.propertyGridFile.TabIndex = 1;
            this.propertyGridFile.ToolbarVisible = false;
            // 
            // tabPageSecurity
            // 
            this.tabPageSecurity.Controls.Add(this.securityEditor1);
            this.tabPageSecurity.Location = new System.Drawing.Point(4, 22);
            this.tabPageSecurity.Name = "tabPageSecurity";
            this.tabPageSecurity.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSecurity.Size = new System.Drawing.Size(471, 367);
            this.tabPageSecurity.TabIndex = 1;
            this.tabPageSecurity.Text = "Security";
            this.tabPageSecurity.UseVisualStyleBackColor = true;
            // 
            // securityEditor1
            // 
            this.securityEditor1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.securityEditor1.Location = new System.Drawing.Point(3, 3);
            this.securityEditor1.Name = "securityEditor1";
            this.securityEditor1.Size = new System.Drawing.Size(465, 361);
            this.securityEditor1.TabIndex = 0;
            this.securityEditor1.ACLChanged += new System.EventHandler(this.httpHeadersEditor_HttpHeadersChanged);
            // 
            // tabPageHeaders
            // 
            this.tabPageHeaders.Controls.Add(this.httpHeadersEditor);
            this.tabPageHeaders.Location = new System.Drawing.Point(4, 22);
            this.tabPageHeaders.Name = "tabPageHeaders";
            this.tabPageHeaders.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageHeaders.Size = new System.Drawing.Size(471, 367);
            this.tabPageHeaders.TabIndex = 1;
            this.tabPageHeaders.Text = "HTTP Headers";
            this.tabPageHeaders.UseVisualStyleBackColor = true;
            // 
            // httpHeadersEditor
            // 
            this.httpHeadersEditor.Dock = System.Windows.Forms.DockStyle.Fill;
            this.httpHeadersEditor.Location = new System.Drawing.Point(3, 3);
            this.httpHeadersEditor.Name = "httpHeadersEditor";
            this.httpHeadersEditor.Size = new System.Drawing.Size(465, 361);
            this.httpHeadersEditor.TabIndex = 0;
            this.httpHeadersEditor.HttpHeadersChanged += new System.EventHandler(this.httpHeadersEditor_HttpHeadersChanged);
            // 
            // EntryForm
            // 
            this.AcceptButton = this.buttonOK;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.buttonCancel;
            this.ClientSize = new System.Drawing.Size(479, 434);
            this.Controls.Add(this.tabControlProperties);
            this.Controls.Add(this.panel1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(487, 464);
            this.Name = "EntryForm";
            this.ShowInTaskbar = false;
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Show;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "<Entry Name>";
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.tabControlProperties.ResumeLayout(false);
            this.tabPageGeneral.ResumeLayout(false);
            this.tabPageSecurity.ResumeLayout(false);
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
        private System.Windows.Forms.CheckBox checkBoxSubfolders;
        private AmazonS3Commander.Controls.HttpHeadersEditor httpHeadersEditor;
        private AmazonS3Commander.Controls.SecurityEditor securityEditor1;
    }
}