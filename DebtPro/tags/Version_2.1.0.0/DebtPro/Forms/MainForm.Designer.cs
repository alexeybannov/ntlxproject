namespace CI.Debt.Forms
{
    partial class MainForm
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
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
			System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
			this.mainMenuStrip = new System.Windows.Forms.MenuStrip();
			this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemCopy = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemImport = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripMenuItemExport = new System.Windows.Forms.ToolStripMenuItem();
			this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.bookToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.subjectsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.classifiersToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.reportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.report1ToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
			this.exportFolderBrowser = new System.Windows.Forms.FolderBrowserDialog();
			this.panel1 = new System.Windows.Forms.Panel();
			this.buttonExport = new System.Windows.Forms.Button();
			this.initWorker = new System.ComponentModel.BackgroundWorker();
			this.openFileDialogImport = new System.Windows.Forms.OpenFileDialog();
			this.rowsDataGrid = new CI.Debt.Forms.DataGridViewEx();
			this.ClassifierColumn = new CI.Debt.Forms.ClassifierColumn();
			this.AmountColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.Amount2Column = new System.Windows.Forms.DataGridViewTextBoxColumn();
			this.SubjectColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
			this.debtDocProperties = new CI.Debt.Forms.DebtPropertiesControl();
			this.mainMenuStrip.SuspendLayout();
			this.panel1.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.rowsDataGrid)).BeginInit();
			this.SuspendLayout();
			// 
			// mainMenuStrip
			// 
			this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.bookToolStripMenuItem,
            this.reportToolStripMenuItem,
            this.helpToolStripMenuItem});
			this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
			this.mainMenuStrip.Name = "mainMenuStrip";
			this.mainMenuStrip.Size = new System.Drawing.Size(964, 24);
			this.mainMenuStrip.TabIndex = 0;
			this.mainMenuStrip.Text = "menuStrip1";
			// 
			// fileToolStripMenuItem
			// 
			this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripMenuItemCopy,
            this.toolStripMenuItemImport,
            this.toolStripMenuItemExport,
            this.settingsToolStripMenuItem,
            this.toolStripSeparator1,
            this.exitToolStripMenuItem});
			this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
			this.fileToolStripMenuItem.Size = new System.Drawing.Size(51, 20);
			this.fileToolStripMenuItem.Text = "&Файл";
			// 
			// toolStripMenuItemCopy
			// 
			this.toolStripMenuItemCopy.Name = "toolStripMenuItemCopy";
			this.toolStripMenuItemCopy.Size = new System.Drawing.Size(170, 22);
			this.toolStripMenuItemCopy.Text = "&Копировать";
			this.toolStripMenuItemCopy.Click += new System.EventHandler(this.toolStripMenuItemCopy_Click);
			// 
			// toolStripMenuItemImport
			// 
			this.toolStripMenuItemImport.Name = "toolStripMenuItemImport";
			this.toolStripMenuItemImport.Size = new System.Drawing.Size(170, 22);
			this.toolStripMenuItemImport.Text = "Импорт";
			this.toolStripMenuItemImport.Click += new System.EventHandler(this.toolStripMenuItemImport_Click);
			// 
			// toolStripMenuItemExport
			// 
			this.toolStripMenuItemExport.Name = "toolStripMenuItemExport";
			this.toolStripMenuItemExport.Size = new System.Drawing.Size(170, 22);
			this.toolStripMenuItemExport.Text = "Экспорт";
			this.toolStripMenuItemExport.Click += new System.EventHandler(this.buttonExport_Click);
			// 
			// settingsToolStripMenuItem
			// 
			this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
			this.settingsToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
			this.settingsToolStripMenuItem.Text = "&Настройки";
			this.settingsToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(167, 6);
			// 
			// exitToolStripMenuItem
			// 
			this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
			this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
			this.exitToolStripMenuItem.Size = new System.Drawing.Size(170, 22);
			this.exitToolStripMenuItem.Text = "&Выход";
			this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
			// 
			// bookToolStripMenuItem
			// 
			this.bookToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.subjectsToolStripMenuItem,
            this.classifiersToolStripMenuItem});
			this.bookToolStripMenuItem.Name = "bookToolStripMenuItem";
			this.bookToolStripMenuItem.Size = new System.Drawing.Size(97, 20);
			this.bookToolStripMenuItem.Text = "&Справочники";
			// 
			// subjectsToolStripMenuItem
			// 
			this.subjectsToolStripMenuItem.Name = "subjectsToolStripMenuItem";
			this.subjectsToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
			this.subjectsToolStripMenuItem.Text = "&Учреждения";
			this.subjectsToolStripMenuItem.Click += new System.EventHandler(this.subjectsToolStripMenuItem_Click);
			// 
			// classifiersToolStripMenuItem
			// 
			this.classifiersToolStripMenuItem.Name = "classifiersToolStripMenuItem";
			this.classifiersToolStripMenuItem.Size = new System.Drawing.Size(188, 22);
			this.classifiersToolStripMenuItem.Text = "&Классификаторы";
			this.classifiersToolStripMenuItem.Click += new System.EventHandler(this.classifiersToolStripMenuItem_Click);
			// 
			// reportToolStripMenuItem
			// 
			this.reportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.report1ToolStripMenuItem});
			this.reportToolStripMenuItem.Name = "reportToolStripMenuItem";
			this.reportToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
			this.reportToolStripMenuItem.Text = "&Отчеты";
			// 
			// report1ToolStripMenuItem
			// 
			this.report1ToolStripMenuItem.Name = "report1ToolStripMenuItem";
			this.report1ToolStripMenuItem.Size = new System.Drawing.Size(181, 22);
			this.report1ToolStripMenuItem.Text = "Задолженность";
			this.report1ToolStripMenuItem.Click += new System.EventHandler(this.report1ToolStripMenuItem_Click);
			// 
			// helpToolStripMenuItem
			// 
			this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem});
			this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
			this.helpToolStripMenuItem.Size = new System.Drawing.Size(68, 20);
			this.helpToolStripMenuItem.Text = "&Помощь";
			// 
			// aboutToolStripMenuItem
			// 
			this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
			this.aboutToolStripMenuItem.Size = new System.Drawing.Size(165, 22);
			this.aboutToolStripMenuItem.Text = "&О программе";
			this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
			// 
			// exportFolderBrowser
			// 
			this.exportFolderBrowser.Description = "Выберите каталог экспорта";
			this.exportFolderBrowser.RootFolder = System.Environment.SpecialFolder.MyComputer;
			this.exportFolderBrowser.SelectedPath = "A:";
			// 
			// panel1
			// 
			this.panel1.Controls.Add(this.buttonExport);
			this.panel1.Controls.Add(this.debtDocProperties);
			this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
			this.panel1.Location = new System.Drawing.Point(0, 24);
			this.panel1.Name = "panel1";
			this.panel1.Size = new System.Drawing.Size(964, 34);
			this.panel1.TabIndex = 2;
			// 
			// buttonExport
			// 
			this.buttonExport.Location = new System.Drawing.Point(523, 7);
			this.buttonExport.Name = "buttonExport";
			this.buttonExport.Size = new System.Drawing.Size(120, 23);
			this.buttonExport.TabIndex = 2;
			this.buttonExport.Text = "&Экспорт";
			this.buttonExport.UseVisualStyleBackColor = true;
			this.buttonExport.Click += new System.EventHandler(this.buttonExport_Click);
			// 
			// initWorker
			// 
			this.initWorker.DoWork += new System.ComponentModel.DoWorkEventHandler(this.initBooksWorker_DoWork);
			this.initWorker.RunWorkerCompleted += new System.ComponentModel.RunWorkerCompletedEventHandler(this.initBooksWorker_RunWorkerCompleted);
			// 
			// openFileDialogImport
			// 
			this.openFileDialogImport.DefaultExt = "xml";
			this.openFileDialogImport.Filter = "Файл импорта задолженности|*.xml|Все файлы|*.*";
			this.openFileDialogImport.Title = "Выберите файл импорта задолженности";
			// 
			// rowsDataGrid
			// 
			this.rowsDataGrid.AllowUserToResizeRows = false;
			this.rowsDataGrid.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
			this.rowsDataGrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
			this.rowsDataGrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.ClassifierColumn,
            this.AmountColumn,
            this.Amount2Column,
            this.SubjectColumn});
			this.rowsDataGrid.Dock = System.Windows.Forms.DockStyle.Fill;
			this.rowsDataGrid.Location = new System.Drawing.Point(0, 58);
			this.rowsDataGrid.Name = "rowsDataGrid";
			this.rowsDataGrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
			this.rowsDataGrid.ShowCellErrors = false;
			this.rowsDataGrid.ShowRowErrors = false;
			this.rowsDataGrid.Size = new System.Drawing.Size(964, 559);
			this.rowsDataGrid.TabIndex = 1;
			this.rowsDataGrid.VirtualMode = true;
			this.rowsDataGrid.UserDeletingRow += new System.Windows.Forms.DataGridViewRowCancelEventHandler(this.rowsDataGrid_UserDeletingRow);
			this.rowsDataGrid.CellValueNeeded += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.rowsDataGrid_CellValueNeeded);
			this.rowsDataGrid.CellValuePushed += new System.Windows.Forms.DataGridViewCellValueEventHandler(this.rowsDataGrid_CellValuePushed);
			this.rowsDataGrid.NewRowNeeded += new System.Windows.Forms.DataGridViewRowEventHandler(this.rowsDataGrid_NewRowNeeded);
			// 
			// ClassifierColumn
			// 
			dataGridViewCellStyle1.Font = new System.Drawing.Font("Courier New", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
			this.ClassifierColumn.DefaultCellStyle = dataGridViewCellStyle1;
			this.ClassifierColumn.HeaderText = "Классификатор";
			this.ClassifierColumn.Name = "ClassifierColumn";
			this.ClassifierColumn.Width = 330;
			// 
			// AmountColumn
			// 
			dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
			dataGridViewCellStyle2.ForeColor = System.Drawing.Color.Navy;
			dataGridViewCellStyle2.Format = "N2";
			dataGridViewCellStyle2.NullValue = "0,00";
			this.AmountColumn.DefaultCellStyle = dataGridViewCellStyle2;
			this.AmountColumn.HeaderText = "Сумма";
			this.AmountColumn.Name = "AmountColumn";
			this.AmountColumn.Width = 130;
			// 
			// Amount2Column
			// 
			dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
			dataGridViewCellStyle3.Format = "N2";
			dataGridViewCellStyle3.NullValue = "0,00";
			this.Amount2Column.DefaultCellStyle = dataGridViewCellStyle3;
			this.Amount2Column.HeaderText = "Просроченная задолженность";
			this.Amount2Column.Name = "Amount2Column";
			this.Amount2Column.Width = 130;
			// 
			// SubjectColumn
			// 
			this.SubjectColumn.HeaderText = "Субъект";
			this.SubjectColumn.MaxDropDownItems = 40;
			this.SubjectColumn.Name = "SubjectColumn";
			this.SubjectColumn.Width = 600;
			// 
			// debtDocProperties
			// 
			this.debtDocProperties.DebtType = CI.Debt.Domain.DebtType.Кредиторская;
			this.debtDocProperties.Location = new System.Drawing.Point(3, 3);
			this.debtDocProperties.Month = 1;
			this.debtDocProperties.Name = "debtDocProperties";
			this.debtDocProperties.Size = new System.Drawing.Size(480, 27);
			this.debtDocProperties.TabIndex = 1;
			this.debtDocProperties.Year = 2009;
			this.debtDocProperties.DebtPropertiesChanged += new System.EventHandler(this.debtDocProperties_DebtPropertiesChanged);
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(964, 617);
			this.Controls.Add(this.rowsDataGrid);
			this.Controls.Add(this.panel1);
			this.Controls.Add(this.mainMenuStrip);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.mainMenuStrip;
			this.MinimumSize = new System.Drawing.Size(600, 400);
			this.Name = "MainForm";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.Text = "Кредиторская и дебиторская задолженности учреждений";
			this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
			this.Shown += new System.EventHandler(this.MainForm_Shown);
			this.mainMenuStrip.ResumeLayout(false);
			this.mainMenuStrip.PerformLayout();
			this.panel1.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.rowsDataGrid)).EndInit();
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

		private System.Windows.Forms.MenuStrip mainMenuStrip;
		private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem bookToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem subjectsToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem classifiersToolStripMenuItem;
		private DataGridViewEx rowsDataGrid;
        private System.Windows.Forms.FolderBrowserDialog exportFolderBrowser;
		private System.Windows.Forms.ToolStripMenuItem reportToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem report1ToolStripMenuItem;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Button buttonExport;
		private DebtPropertiesControl debtDocProperties;
		private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
		private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
		private System.ComponentModel.BackgroundWorker initWorker;
		private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemCopy;
		private ClassifierColumn ClassifierColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn AmountColumn;
		private System.Windows.Forms.DataGridViewTextBoxColumn Amount2Column;
		private System.Windows.Forms.DataGridViewComboBoxColumn SubjectColumn;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemImport;
		private System.Windows.Forms.ToolStripMenuItem toolStripMenuItemExport;
		private System.Windows.Forms.OpenFileDialog openFileDialogImport;
    }
}

