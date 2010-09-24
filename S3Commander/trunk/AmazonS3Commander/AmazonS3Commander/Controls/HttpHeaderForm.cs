using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using AmazonS3Commander.Utils;

namespace AmazonS3Commander.Controls
{
    public partial class HttpHeaderForm : Form
    {
        [Browsable(false)]
        public string HttpHeader
        {
            get { return comboBoxHeader.Text; }
        }

        [Browsable(false)]
        public string HttpHeaderValue
        {
            get { return comboBoxValue.Text; }
        }

        public HttpHeaderForm(IEnumerable<string> existsHeaders)
        {
            InitializeComponent();

            Text = "Add Http Header";
            foreach (var header in HttpHeaderProvider.GetEditableHeaders())
            {
                if (existsHeaders != null && !existsHeaders.Contains(header, StringComparer.InvariantCultureIgnoreCase))
                {
                    comboBoxHeader.Items.Add(header);
                }
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

            var values = HttpHeaderProvider.GetHeaderValues(comboBoxHeader.Text);
            if (0 < values.Length)
            {
                comboBoxValue.Items.Clear();
                foreach (var value in values)
                {
                    comboBoxValue.Items.Add(value);
                }
            }
            else
            {
                if (0 < comboBoxValue.Items.Count)
                {
                    comboBoxValue.Items.Clear();
                }
            }
        }
    }
}
