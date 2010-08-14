using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using AmazonS3Commander.Properties;

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


        public AccountForm(string name)
        {
            InitializeComponent();

            Icon = Resources.NewAccountIcon;
            Text = Resources.NewAccount;
            MinimumSize = Size;
            textBox1.Text = name ?? string.Empty;
            textBox1.SelectionStart = textBox1.TextLength;
        }

        public AccountForm(string name, AccountInfo info)
            : this(name)
        {
            if (info == null) throw new ArgumentNullException("info");

            Icon = Resources.AccountIcon;
            Text = Resources.EditAccount;
            textBox1.Enabled = false;
            textBox2.Text = info.AccessKey ?? string.Empty;
            textBox3.Text = info.SecretKey ?? string.Empty;
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
