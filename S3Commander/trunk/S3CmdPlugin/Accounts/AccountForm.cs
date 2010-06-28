using System;
using System.Windows.Forms;
using S3CmdPlugin.Resources;

namespace S3CmdPlugin.Accounts
{
	public partial class AccountForm : Form
	{
		public string AccountName
		{
			get { return textBox1.Text; }
			set
			{
				textBox1.Text = value ?? string.Empty;
				if (textBox1.Text != string.Empty) textBox2.Focus();
			}
		}

		public string AccountAccessKey
		{
			get { return textBox2.Text; }
			set
			{
				textBox2.Text = value ?? string.Empty;
				if (textBox2.Text != string.Empty) textBox3.Focus();
			}
		}

		public string AccountSecretKey
		{
			get { return textBox3.Text; }
			set
			{
				textBox3.Text = value ?? string.Empty;
				if (textBox3.Text != string.Empty) buttonOK.Focus();
			}
		}


		public AccountForm()
		{
			InitializeComponent();
			Text = PluginResources.NewAccount;
		}

		public AccountForm(string name, string accessKey, string secretKey)
			: this()
		{
			AccountName = name;
			AccountAccessKey = accessKey;
			AccountSecretKey = secretKey;

			Text = PluginResources.EditAccount;
			textBox1.Enabled = false;
			textBox2.Focus();
			textBox2.SelectionStart = textBox2.TextLength;
		}

		private void TextBox1TextChanged(object sender, EventArgs e)
		{
			buttonOK.Enabled = ((TextBox)sender).TextLength != 0;
		}
	}
}
