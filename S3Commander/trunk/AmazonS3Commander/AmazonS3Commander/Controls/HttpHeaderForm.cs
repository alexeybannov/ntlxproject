using System;
using System.Windows.Forms;
using AmazonS3Commander.Files;

namespace AmazonS3Commander.Controls
{
    public partial class HttpHeaderForm : Form
    {
        public string Header
        {
            get { return comboBoxHeader.Text; }
        }

        public string Value
        {
            get { return comboBoxValue.Text; }
        }


        public HttpHeaderForm()
        {
            InitializeComponent();

            Text = "Add Http Header";
            foreach (var header in HttpHeaderProvider.GetEditableHeaders())
            {
                comboBoxHeader.Items.Add(header);
            }
        }

        public HttpHeaderForm(string header, string value)
        {
            if (string.IsNullOrEmpty(header)) throw new ArgumentNullException("header");

            InitializeComponent();

            Text = "Edit Http Header";
            comboBoxHeader.Text = header;
            comboBoxHeader.Enabled = false;
            comboBoxValue.Text = value ?? string.Empty;
        }

        private void comboBoxHeader_TextChanged(object sender, EventArgs e)
        {
            buttonOK.Enabled = 0 < comboBoxHeader.Text.Trim().Length;
        }
    }
}
