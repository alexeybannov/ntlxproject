using System;
using System.Windows.Forms;

namespace AmazonS3Commander.Controls
{
    partial class SecurityAddForm : Form
    {
        public string UserEmailOrId
        {
            get { return textBox1.Text; }
        }


        public SecurityAddForm()
        {
            InitializeComponent();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            buttonOK.Enabled = 0 < ((TextBox)sender).TextLength;
        }
    }
}
