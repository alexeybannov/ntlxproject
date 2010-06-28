using System;
using System.Windows.Forms;
using S3CmdPlugin.Resources;

namespace S3CmdPlugin.Accouts
{
	public partial class AccountForm : Form
	{
		public string AccountName
		{
			get { return textBox1.Text; }
			set { textBox1.Text = value ?? string.Empty; }
		}

		public string AccountAccessKey
		{
			get { return textBox2.Text; }
			set { textBox2.Text = value ?? string.Empty; }
		}

		public string AccountSecretKey
		{
			get { return textBox3.Text; }
			set { textBox3.Text = value ?? string.Empty; }
		}


		public AccountForm()
		{
			InitializeComponent();
			Text = PluginResources.NewAccount;
		}

		public AccountForm(string name, string accessKey, string secretKey)
		{
			AccountName = name;
			AccountAccessKey = accessKey;
			AccountSecretKey = secretKey;
			Text = PluginResources.EditAccount;
		}

        private void TextBox1TextChanged(object sender, EventArgs e)
        {
            buttonOK.Enabled = ((TextBox)sender).TextLength != 0;
        }
	}
}
