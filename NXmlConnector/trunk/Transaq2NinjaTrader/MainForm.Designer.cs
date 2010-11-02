namespace Transaq2NinjaTrader
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
            this.tabPageLog = new System.Windows.Forms.TabPage();
            this.richTextBoxLog = new System.Windows.Forms.RichTextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            this.buttonClearLog = new System.Windows.Forms.Button();
            this.tabPageSettings = new System.Windows.Forms.TabPage();
            this.buttonConnect = new System.Windows.Forms.Button();
            this.numericUpDownPort = new System.Windows.Forms.NumericUpDown();
            this.checkBoxAutoPos = new System.Windows.Forms.CheckBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.textBoxLogin = new System.Windows.Forms.TextBox();
            this.textBoxServer = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.tabPageSecurities = new System.Windows.Forms.TabPage();
            this.dataGridViewSecurities = new System.Windows.Forms.DataGridView();
            this.SecSelect = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.SecCode = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SecName = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SecType = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SecMarket = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.SecId = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.panel2 = new System.Windows.Forms.Panel();
            this.label7 = new System.Windows.Forms.Label();
            this.comboBoxKind = new System.Windows.Forms.ComboBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.dateTimePickerHistoryEnd = new System.Windows.Forms.DateTimePicker();
            this.dateTimePickerHistoryStart = new System.Windows.Forms.DateTimePicker();
            this.buttonHistory = new System.Windows.Forms.Button();
            this.buttonUnsubscribe = new System.Windows.Forms.Button();
            this.buttonSubscribe = new System.Windows.Forms.Button();
            this.tabPageData = new System.Windows.Forms.TabPage();
            this.label9 = new System.Windows.Forms.Label();
            this.listView1 = new System.Windows.Forms.ListView();
            this.columnHeaderQuSec = new System.Windows.Forms.ColumnHeader();
            this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
            this.listViewHistory = new System.Windows.Forms.ListView();
            this.columnHeaderHisName = new System.Windows.Forms.ColumnHeader();
            this.columnHeaderHisCount = new System.Windows.Forms.ColumnHeader();
            this.label8 = new System.Windows.Forms.Label();
            this.tabPageLog.SuspendLayout();
            this.panel1.SuspendLayout();
            this.tabPageSettings.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPort)).BeginInit();
            this.tabControl.SuspendLayout();
            this.tabPageSecurities.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSecurities)).BeginInit();
            this.panel2.SuspendLayout();
            this.tabPageData.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabPageLog
            // 
            this.tabPageLog.Controls.Add(this.richTextBoxLog);
            this.tabPageLog.Controls.Add(this.panel1);
            this.tabPageLog.Location = new System.Drawing.Point(4, 22);
            this.tabPageLog.Name = "tabPageLog";
            this.tabPageLog.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageLog.Size = new System.Drawing.Size(789, 461);
            this.tabPageLog.TabIndex = 1;
            this.tabPageLog.Text = "Лог";
            this.tabPageLog.UseVisualStyleBackColor = true;
            // 
            // richTextBoxLog
            // 
            this.richTextBoxLog.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richTextBoxLog.Location = new System.Drawing.Point(3, 3);
            this.richTextBoxLog.Name = "richTextBoxLog";
            this.richTextBoxLog.Size = new System.Drawing.Size(783, 426);
            this.richTextBoxLog.TabIndex = 1;
            this.richTextBoxLog.Text = "";
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.buttonClearLog);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(3, 429);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(783, 29);
            this.panel1.TabIndex = 0;
            // 
            // buttonClearLog
            // 
            this.buttonClearLog.Location = new System.Drawing.Point(5, 3);
            this.buttonClearLog.Name = "buttonClearLog";
            this.buttonClearLog.Size = new System.Drawing.Size(75, 23);
            this.buttonClearLog.TabIndex = 0;
            this.buttonClearLog.Text = "Очистить";
            this.buttonClearLog.UseVisualStyleBackColor = true;
            this.buttonClearLog.Click += new System.EventHandler(this.buttonClearLog_Click);
            // 
            // tabPageSettings
            // 
            this.tabPageSettings.Controls.Add(this.buttonConnect);
            this.tabPageSettings.Controls.Add(this.numericUpDownPort);
            this.tabPageSettings.Controls.Add(this.checkBoxAutoPos);
            this.tabPageSettings.Controls.Add(this.label4);
            this.tabPageSettings.Controls.Add(this.label3);
            this.tabPageSettings.Controls.Add(this.label2);
            this.tabPageSettings.Controls.Add(this.textBoxPassword);
            this.tabPageSettings.Controls.Add(this.textBoxLogin);
            this.tabPageSettings.Controls.Add(this.textBoxServer);
            this.tabPageSettings.Controls.Add(this.label1);
            this.tabPageSettings.Location = new System.Drawing.Point(4, 22);
            this.tabPageSettings.Name = "tabPageSettings";
            this.tabPageSettings.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSettings.Size = new System.Drawing.Size(789, 461);
            this.tabPageSettings.TabIndex = 0;
            this.tabPageSettings.Text = "Настройки";
            this.tabPageSettings.UseVisualStyleBackColor = true;
            // 
            // buttonConnect
            // 
            this.buttonConnect.Location = new System.Drawing.Point(11, 145);
            this.buttonConnect.Name = "buttonConnect";
            this.buttonConnect.Size = new System.Drawing.Size(120, 23);
            this.buttonConnect.TabIndex = 10;
            this.buttonConnect.Text = "Подключиться";
            this.buttonConnect.UseVisualStyleBackColor = true;
            this.buttonConnect.Click += new System.EventHandler(this.buttonConnect_Click);
            // 
            // numericUpDownPort
            // 
            this.numericUpDownPort.Location = new System.Drawing.Point(76, 32);
            this.numericUpDownPort.Maximum = new decimal(new int[] {
            66000,
            0,
            0,
            0});
            this.numericUpDownPort.Name = "numericUpDownPort";
            this.numericUpDownPort.Size = new System.Drawing.Size(66, 20);
            this.numericUpDownPort.TabIndex = 9;
            this.numericUpDownPort.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.numericUpDownPort.Value = new decimal(new int[] {
            3939,
            0,
            0,
            0});
            // 
            // checkBoxAutoPos
            // 
            this.checkBoxAutoPos.AutoSize = true;
            this.checkBoxAutoPos.Checked = true;
            this.checkBoxAutoPos.CheckState = System.Windows.Forms.CheckState.Checked;
            this.checkBoxAutoPos.Location = new System.Drawing.Point(76, 110);
            this.checkBoxAutoPos.Name = "checkBoxAutoPos";
            this.checkBoxAutoPos.Size = new System.Drawing.Size(180, 17);
            this.checkBoxAutoPos.TabIndex = 8;
            this.checkBoxAutoPos.Text = "Запрос позиций после сделки";
            this.checkBoxAutoPos.UseVisualStyleBackColor = true;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 87);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(48, 13);
            this.label4.TabIndex = 7;
            this.label4.Text = "Пароль:";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 61);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(41, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Логин:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 34);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(35, 13);
            this.label2.TabIndex = 5;
            this.label2.Text = "Порт:";
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(76, 84);
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(140, 20);
            this.textBoxPassword.TabIndex = 4;
            this.textBoxPassword.Text = "E6CNxe";
            this.textBoxPassword.UseSystemPasswordChar = true;
            // 
            // textBoxLogin
            // 
            this.textBoxLogin.Location = new System.Drawing.Point(76, 58);
            this.textBoxLogin.Name = "textBoxLogin";
            this.textBoxLogin.Size = new System.Drawing.Size(140, 20);
            this.textBoxLogin.TabIndex = 3;
            this.textBoxLogin.Text = "TCONN0011";
            // 
            // textBoxServer
            // 
            this.textBoxServer.Location = new System.Drawing.Point(76, 6);
            this.textBoxServer.Name = "textBoxServer";
            this.textBoxServer.Size = new System.Drawing.Size(140, 20);
            this.textBoxServer.TabIndex = 1;
            this.textBoxServer.Text = "195.128.78.60";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 9);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(47, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Сервер:";
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.tabPageSettings);
            this.tabControl.Controls.Add(this.tabPageLog);
            this.tabControl.Controls.Add(this.tabPageSecurities);
            this.tabControl.Controls.Add(this.tabPageData);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(797, 487);
            this.tabControl.TabIndex = 0;
            // 
            // tabPageSecurities
            // 
            this.tabPageSecurities.Controls.Add(this.dataGridViewSecurities);
            this.tabPageSecurities.Controls.Add(this.panel2);
            this.tabPageSecurities.Location = new System.Drawing.Point(4, 22);
            this.tabPageSecurities.Name = "tabPageSecurities";
            this.tabPageSecurities.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageSecurities.Size = new System.Drawing.Size(789, 461);
            this.tabPageSecurities.TabIndex = 2;
            this.tabPageSecurities.Text = "Инструменты";
            this.tabPageSecurities.UseVisualStyleBackColor = true;
            // 
            // dataGridViewSecurities
            // 
            this.dataGridViewSecurities.AllowUserToAddRows = false;
            this.dataGridViewSecurities.AllowUserToDeleteRows = false;
            this.dataGridViewSecurities.AllowUserToResizeRows = false;
            this.dataGridViewSecurities.BackgroundColor = System.Drawing.SystemColors.Window;
            this.dataGridViewSecurities.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.SingleHorizontal;
            this.dataGridViewSecurities.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridViewSecurities.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.SecSelect,
            this.SecCode,
            this.SecName,
            this.SecType,
            this.SecMarket,
            this.SecId});
            this.dataGridViewSecurities.Dock = System.Windows.Forms.DockStyle.Fill;
            this.dataGridViewSecurities.Location = new System.Drawing.Point(3, 3);
            this.dataGridViewSecurities.Name = "dataGridViewSecurities";
            this.dataGridViewSecurities.RowHeadersVisible = false;
            this.dataGridViewSecurities.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dataGridViewSecurities.Size = new System.Drawing.Size(783, 373);
            this.dataGridViewSecurities.TabIndex = 2;
            // 
            // SecSelect
            // 
            this.SecSelect.HeaderText = "";
            this.SecSelect.Name = "SecSelect";
            this.SecSelect.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.SecSelect.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.SecSelect.Width = 30;
            // 
            // SecCode
            // 
            this.SecCode.HeaderText = "Код";
            this.SecCode.Name = "SecCode";
            this.SecCode.ReadOnly = true;
            this.SecCode.Width = 120;
            // 
            // SecName
            // 
            this.SecName.HeaderText = "Название";
            this.SecName.Name = "SecName";
            this.SecName.ReadOnly = true;
            this.SecName.Width = 300;
            // 
            // SecType
            // 
            this.SecType.HeaderText = "Тип";
            this.SecType.Name = "SecType";
            this.SecType.ReadOnly = true;
            // 
            // SecMarket
            // 
            this.SecMarket.HeaderText = "Рынок";
            this.SecMarket.Name = "SecMarket";
            this.SecMarket.ReadOnly = true;
            // 
            // SecId
            // 
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleRight;
            this.SecId.DefaultCellStyle = dataGridViewCellStyle1;
            this.SecId.HeaderText = "Идентификатор";
            this.SecId.Name = "SecId";
            this.SecId.ReadOnly = true;
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.label7);
            this.panel2.Controls.Add(this.comboBoxKind);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.label5);
            this.panel2.Controls.Add(this.dateTimePickerHistoryEnd);
            this.panel2.Controls.Add(this.dateTimePickerHistoryStart);
            this.panel2.Controls.Add(this.buttonHistory);
            this.panel2.Controls.Add(this.buttonUnsubscribe);
            this.panel2.Controls.Add(this.buttonSubscribe);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(3, 376);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(783, 82);
            this.panel2.TabIndex = 1;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(429, 53);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(57, 13);
            this.label7.TabIndex = 9;
            this.label7.Text = "периодом";
            // 
            // comboBoxKind
            // 
            this.comboBoxKind.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxKind.FormattingEnabled = true;
            this.comboBoxKind.Location = new System.Drawing.Point(492, 50);
            this.comboBoxKind.Name = "comboBoxKind";
            this.comboBoxKind.Size = new System.Drawing.Size(156, 21);
            this.comboBoxKind.TabIndex = 8;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(287, 53);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(19, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "по";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(149, 53);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(13, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "с";
            // 
            // dateTimePickerHistoryEnd
            // 
            this.dateTimePickerHistoryEnd.CustomFormat = "dd.MM.yyyy HH:mm";
            this.dateTimePickerHistoryEnd.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerHistoryEnd.Location = new System.Drawing.Point(308, 49);
            this.dateTimePickerHistoryEnd.Name = "dateTimePickerHistoryEnd";
            this.dateTimePickerHistoryEnd.Size = new System.Drawing.Size(115, 20);
            this.dateTimePickerHistoryEnd.TabIndex = 5;
            // 
            // dateTimePickerHistoryStart
            // 
            this.dateTimePickerHistoryStart.CustomFormat = "dd.MM.yyyy HH:mm";
            this.dateTimePickerHistoryStart.Format = System.Windows.Forms.DateTimePickerFormat.Custom;
            this.dateTimePickerHistoryStart.Location = new System.Drawing.Point(169, 49);
            this.dateTimePickerHistoryStart.Name = "dateTimePickerHistoryStart";
            this.dateTimePickerHistoryStart.Size = new System.Drawing.Size(115, 20);
            this.dateTimePickerHistoryStart.TabIndex = 3;
            // 
            // buttonHistory
            // 
            this.buttonHistory.Location = new System.Drawing.Point(5, 48);
            this.buttonHistory.Name = "buttonHistory";
            this.buttonHistory.Size = new System.Drawing.Size(132, 23);
            this.buttonHistory.TabIndex = 2;
            this.buttonHistory.Text = "Запросить данные";
            this.buttonHistory.UseVisualStyleBackColor = true;
            this.buttonHistory.Click += new System.EventHandler(this.buttonHistory_Click);
            // 
            // buttonUnsubscribe
            // 
            this.buttonUnsubscribe.Location = new System.Drawing.Point(143, 6);
            this.buttonUnsubscribe.Name = "buttonUnsubscribe";
            this.buttonUnsubscribe.Size = new System.Drawing.Size(132, 23);
            this.buttonUnsubscribe.TabIndex = 1;
            this.buttonUnsubscribe.Text = "Отписаться";
            this.buttonUnsubscribe.UseVisualStyleBackColor = true;
            this.buttonUnsubscribe.Click += new System.EventHandler(this.buttonUnsubscribe_Click);
            // 
            // buttonSubscribe
            // 
            this.buttonSubscribe.Location = new System.Drawing.Point(5, 6);
            this.buttonSubscribe.Name = "buttonSubscribe";
            this.buttonSubscribe.Size = new System.Drawing.Size(132, 23);
            this.buttonSubscribe.TabIndex = 0;
            this.buttonSubscribe.Text = "Подписаться";
            this.buttonSubscribe.UseVisualStyleBackColor = true;
            this.buttonSubscribe.Click += new System.EventHandler(this.buttonSubscribe_Click);
            // 
            // tabPageData
            // 
            this.tabPageData.Controls.Add(this.label9);
            this.tabPageData.Controls.Add(this.listView1);
            this.tabPageData.Controls.Add(this.listViewHistory);
            this.tabPageData.Controls.Add(this.label8);
            this.tabPageData.Location = new System.Drawing.Point(4, 22);
            this.tabPageData.Name = "tabPageData";
            this.tabPageData.Padding = new System.Windows.Forms.Padding(3);
            this.tabPageData.Size = new System.Drawing.Size(789, 461);
            this.tabPageData.TabIndex = 3;
            this.tabPageData.Text = "Данные";
            this.tabPageData.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 137);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(35, 13);
            this.label9.TabIndex = 3;
            this.label9.Text = "label9";
            // 
            // listView1
            // 
            this.listView1.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderQuSec,
            this.columnHeader1});
            this.listView1.Location = new System.Drawing.Point(6, 153);
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(356, 97);
            this.listView1.TabIndex = 2;
            this.listView1.UseCompatibleStateImageBehavior = false;
            // 
            // columnHeaderQuSec
            // 
            this.columnHeaderQuSec.Width = 150;
            // 
            // listViewHistory
            // 
            this.listViewHistory.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.columnHeaderHisName,
            this.columnHeaderHisCount});
            this.listViewHistory.Location = new System.Drawing.Point(6, 19);
            this.listViewHistory.Name = "listViewHistory";
            this.listViewHistory.Size = new System.Drawing.Size(278, 105);
            this.listViewHistory.TabIndex = 1;
            this.listViewHistory.UseCompatibleStateImageBehavior = false;
            this.listViewHistory.View = System.Windows.Forms.View.Details;
            // 
            // columnHeaderHisName
            // 
            this.columnHeaderHisName.Text = "Инструмент";
            this.columnHeaderHisName.Width = 150;
            // 
            // columnHeaderHisCount
            // 
            this.columnHeaderHisCount.Text = "Свечей";
            this.columnHeaderHisCount.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.columnHeaderHisCount.Width = 100;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(8, 3);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(53, 13);
            this.label8.TabIndex = 0;
            this.label8.Text = "История:";
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(797, 487);
            this.Controls.Add(this.tabControl);
            this.Name = "MainForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Transaq <-> NinjaTrader";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.tabPageLog.ResumeLayout(false);
            this.panel1.ResumeLayout(false);
            this.tabPageSettings.ResumeLayout(false);
            this.tabPageSettings.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDownPort)).EndInit();
            this.tabControl.ResumeLayout(false);
            this.tabPageSecurities.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridViewSecurities)).EndInit();
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.tabPageData.ResumeLayout(false);
            this.tabPageData.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabPage tabPageLog;
        private System.Windows.Forms.RichTextBox richTextBoxLog;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.Button buttonClearLog;
        private System.Windows.Forms.TabPage tabPageSettings;
        private System.Windows.Forms.Button buttonConnect;
        private System.Windows.Forms.NumericUpDown numericUpDownPort;
        private System.Windows.Forms.CheckBox checkBoxAutoPos;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.TextBox textBoxLogin;
        private System.Windows.Forms.TextBox textBoxServer;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TabControl tabControl;
        private System.Windows.Forms.TabPage tabPageSecurities;
        private System.Windows.Forms.DataGridView dataGridViewSecurities;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button buttonHistory;
        private System.Windows.Forms.Button buttonUnsubscribe;
        private System.Windows.Forms.Button buttonSubscribe;
        private System.Windows.Forms.DateTimePicker dateTimePickerHistoryEnd;
        private System.Windows.Forms.DateTimePicker dateTimePickerHistoryStart;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox comboBoxKind;
        private System.Windows.Forms.TabPage tabPageData;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.ListView listViewHistory;
        private System.Windows.Forms.ColumnHeader columnHeaderHisName;
        private System.Windows.Forms.ColumnHeader columnHeaderHisCount;
        private System.Windows.Forms.DataGridViewCheckBoxColumn SecSelect;
        private System.Windows.Forms.DataGridViewTextBoxColumn SecCode;
        private System.Windows.Forms.DataGridViewTextBoxColumn SecName;
        private System.Windows.Forms.DataGridViewTextBoxColumn SecType;
        private System.Windows.Forms.DataGridViewTextBoxColumn SecMarket;
        private System.Windows.Forms.DataGridViewTextBoxColumn SecId;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ColumnHeader columnHeaderQuSec;
        private System.Windows.Forms.ColumnHeader columnHeader1;

    }
}

