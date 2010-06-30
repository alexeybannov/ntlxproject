using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Net;

namespace S3CmdPlugin.Config
{
	public partial class ConfigForm : Form
	{
		public ConfigForm()
		{
			InitializeComponent();
			var proxy = WebRequest.GetSystemWebProxy();
			if (proxy != null)
			{
				checkBox1.Checked = true;
				textBox3.Text = proxy.GetProxy(new Uri(textBox1.Text)).ToString();
			}
		}

		private void checkBox1_CheckedChanged(object sender, EventArgs e)
		{
			label4.Enabled = label5.Enabled = label6.Enabled =
				textBox3.Enabled = textBox4.Enabled = textBox5.Enabled =
					checkBox1.Checked;
		}
	}
}
