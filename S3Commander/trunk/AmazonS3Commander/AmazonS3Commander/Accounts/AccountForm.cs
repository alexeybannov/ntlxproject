using System;
using System.Diagnostics;
using System.Windows.Forms;
using AmazonS3Commander.Properties;

namespace AmazonS3Commander.Accounts
{
	partial class AccountForm : Form
	{
		public string AccountName
		{
            [DebuggerStepThrough]
			get { return textBox1.Text; }
		}

		public AccountInfo AccountInfo
		{
            [DebuggerStepThrough]
			get { return new AccountInfo(textBox2.Text, textBox3.Text); }
		}


		public AccountForm(string name)
		{
			InitializeComponent();

            Icon = Resources.AccountIcon;
			Text = Resources.NewAccount;
            MinimumSize = Size;
            textBox1.Text = name ?? string.Empty;
            textBox1.SelectionStart = textBox1.TextLength;
		}

		public AccountForm(string name, AccountInfo info)
			: this(name)
		{
			if (info == null) throw new ArgumentNullException("info");

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
	}
}
