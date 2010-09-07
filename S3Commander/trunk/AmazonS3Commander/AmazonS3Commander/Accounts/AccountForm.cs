using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using AmazonS3Commander.Resources;

namespace AmazonS3Commander.Accounts
{
    partial class AccountForm : Form
    {
        [Browsable(false)]
        public string AccountName
        {
            [DebuggerStepThrough]
            get { return textBox1.Text; }
        }

        [Browsable(false)]
        public AccountInfo AccountInfo
        {
            [DebuggerStepThrough]
            get { return new AccountInfo(textBox2.Text, textBox3.Text); }
        }


        public AccountForm()
        {
            InitializeComponent();
            MinimumSize = Size;
        }

        public AccountForm(string name)
            : this()
        {
            Icon = RS.NewAccountIcon;
            Text = RS.NewAccount;

            textBox1.Text = name ?? string.Empty;
            textBox1.SelectionStart = textBox1.TextLength;
        }

        public AccountForm(string name, AccountInfo info)
            : this()
        {
            if (info == null) throw new ArgumentNullException("info");

            Icon = RS.AccountIcon;
            Text = RS.EditAccount;

            textBox1.Text = name ?? string.Empty;
            textBox2.Text = info.AccessKey ?? string.Empty;
            textBox3.Text = info.SecretKey ?? string.Empty;
            textBox1.Enabled = false;
        }

        private void TextBox1TextChanged(object sender, EventArgs e)
        {
            buttonOK.Enabled = ((TextBox)sender).TextLength != 0;
        }

        private void AccountForm_Shown(object sender, EventArgs e)
        {
            if (0 < textBox1.TextLength || 0 < textBox3.TextLength) textBox2.Focus();
            else if (0 < textBox2.TextLength) textBox3.Focus();
        }

        private void textBox_Pasted(object sender, EventArgs e)
        {
            if (sender == textBox1) textBox2.Focus();
            if (sender == textBox2) textBox3.Focus();
            if (sender == textBox3) buttonOK.Focus();
        }
    }
}
